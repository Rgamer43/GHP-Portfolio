import { Socket } from 'socket.io';
import { ObjectId } from 'mongodb';
import { addComment } from '../database/posts';
import { ClientCmds } from '../types';

export async function postComment(socket: Socket, post: ObjectId, comment: string, user: ObjectId) {
    const MIN_CHARS = process.env.COMMENT_CHAR_MIN as unknown as number, MAX_CHARS = process.env.COMMENT_CHAR_MAX as unknown as number
    if(comment.length >= MIN_CHARS && comment.length <= MAX_CHARS) {
        const posted: boolean = await addComment(post, user, comment)

        if(!posted) socket.emit("seterror", "You've already posted that comment. Post a new comment.")
        else socket.send(ClientCmds.reloadPost)

        return
    } else if(comment.length <= MAX_CHARS ) socket.emit("seterror", "Comments must be at least " + MIN_CHARS + " characters.")
    else socket.emit("seterror", "Comments must be less than " + MAX_CHARS + " characters.")
}