import { ObjectId } from 'mongodb';
import { Socket } from 'socket.io';
import { deletePostByID } from '../database/posts';
import { getIsAdminByEmail } from '../database/users';

export async function deletePost(socket: Socket, post: ObjectId) {
    if(await getIsAdminByEmail(socket.data.email)) {
        console.log("Deleting post " + post)
        deletePostByID(post)
    } else console.log(socket.data.email + " is not an admin, but tried to delete post " + post + "!!")
}