import { Socket } from "socket.io";
import { ObjectId } from 'mongodb';
import { addDislike, addLike, hasDisliked, hasLiked, removeDislike, removeLike } from "../database/posts";
import { getIDByEmail } from "../database/users";
import { ClientCmds } from '../types';

export async function dislikePost(socket: Socket, post: string) {
    let id = ObjectId.createFromHexString(post)
    if(await hasDisliked(id, await getIDByEmail(socket.data.email))) removeDislike(id, await getIDByEmail(socket.data.email))
    else addDislike(id, await getIDByEmail(socket.data.email))
    socket.send(ClientCmds.reloadPost)
}