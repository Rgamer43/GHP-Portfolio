import { Socket } from 'socket.io';
import { ObjectId } from 'mongodb';
import { addView, getDislikes, getLikes, getPostByID, getViews, hasDisliked, hasLiked } from '../database/posts';
import { getIDByEmail, getUsernameByID } from '../database/users';

export async function getPost(socket: Socket, id: ObjectId) {
    let post = await getPostByID(id)
    post!.owner = await getUsernameByID(post!.owner)
    
    if(socket.data.email !== undefined) {
        post!.views = await addView(id, await getIDByEmail(socket.data.email))
        if(await hasLiked(id, await getIDByEmail(socket.data.email))) post!.hasLiked = true
        else post!.hasLiked = false

        if(await hasDisliked(id, await getIDByEmail(socket.data.email))) post!.hasDisliked = true
        else post!.hasDisliked = false
    } else post!.views = await getViews(id)
    
    post!.likes = await getLikes(id)
    post!.dislikes = await getDislikes(id)

    
    if(post!.time !== undefined) {
        post!.time = new Date(post!.time).toLocaleDateString()
    } else post!.time = "before the dawn of time"

    if(post!.comments !== undefined) {
        for (let i = 0; i < post!.comments.length; i++) {
            const element = post!.comments[i];
            element.user = await getUsernameByID(element.user)
            if(element.time !== undefined) {
                element.time = new Date(element.time).toLocaleTimeString() + " " + new Date(element.time).toLocaleDateString()
            } else element.time = "Before the dawn of time"
        }
    } else post!.comments = []

    const body = post!.body
    post!.body = ""

    socket.emit("setpostdata", post)
    socket.emit("setbody", body)
}