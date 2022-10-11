import { Socket } from "socket.io";
import { ObjectId } from 'mongodb';
import { addLike, hasLiked, removeLike } from "../database/posts";
import { getIDByEmail } from "../database/users";
import { ClientCmds } from '../types';

export async function likePost(socket: Socket, post: string) {
    let id = ObjectId.createFromHexString(post)
    if(await hasLiked(id, await getIDByEmail(socket.data.email))) removeLike(id, await getIDByEmail(socket.data.email))
    else addLike(id, await getIDByEmail(socket.data.email))
    socket.send(ClientCmds.reloadPost)
}