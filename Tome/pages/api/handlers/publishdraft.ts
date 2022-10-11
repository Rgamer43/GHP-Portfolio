import { ObjectId } from 'mongodb';
import { Socket } from 'socket.io';
import { ownsDraft } from '../database/drafts';
import { addPost } from '../database/posts';
import { getIDByEmail } from '../database/users';
import { ClientCmds } from '../types';
export async function publishDraft(socket: Socket, draft: any) {
    const MIN_WORDS: number = process.env.WORD_MIN as unknown as number,
        MAX_WORDS: number = process.env.WORD_MAX as unknown as number 
    let wordCount: number = (draft.body as string).split(" ").length
    draft._id = ObjectId.createFromHexString(draft._id)

    if(wordCount < MIN_WORDS || wordCount > MAX_WORDS || !(await ownsDraft(draft._id, await getIDByEmail(socket.data.email)))) {
        console.log("Rejected draft")
        socket.emit("wordcounterror", [wordCount, MIN_WORDS, MAX_WORDS])
    } else {
        console.log("Publishing draft...")
        socket.emit("gotopost", await addPost(draft, await getIDByEmail(socket.data.email)))
    }
}