import { ObjectId } from 'mongodb';
import { Socket } from 'socket.io';
import { deleteCommentByID, deletePostByID } from '../database/posts';
import { getIsAdminByEmail } from '../database/users';

export async function deleteComment(socket: Socket, post: ObjectId, id: number) {
    if(await getIsAdminByEmail(socket.data.email)) {
        console.log("Deleting comment " + id + " on " + post)
        deleteCommentByID(post, id)
    } else console.log(socket.data.email + " is not an admin, but tried to delete comment " + id + " on post " + post + "!!")
}