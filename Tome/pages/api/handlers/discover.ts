import { Socket } from 'socket.io';
import { getDislikes, getExplorePosts, getHotPosts, getLikes, getRecentPosts, getTopPosts, getViews } from '../database/posts';
import { getUsernameByID } from '../database/users';

export async function discover(socket: Socket, mode: string) {
    let posts: any[]

    if(mode === "top") posts = await getTopPosts()
    else if (mode === "hot") posts = await getHotPosts()
    else if (mode === "recent") posts = await getRecentPosts()
    else posts = await getExplorePosts()

    for (let i = 0; i < posts.length; i++) {
        const element = posts[i];
        element.owner = await getUsernameByID(element!.owner)
        // element.views = await getViews(element._id)
        // element.likes = await getLikes(element._id)
        // element.dislikes = await getDislikes(element._id)
        element.body = element.body.slice(0, process.env.CARD_BODY_MAX as unknown as number)
    }

    socket.emit("discover", [ mode, posts ])
}