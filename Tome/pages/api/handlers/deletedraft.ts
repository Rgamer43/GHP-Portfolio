import { Socket } from 'socket.io';
import { ObjectId } from 'mongodb';
import { ownsDraft, removeDraft } from '../database/drafts';
import { getIDByEmail } from '../database/users';
import { ClientCmds } from '../types';

export async function deleteDraft(socket: Socket, id: ObjectId) {
    if(await ownsDraft(id, await getIDByEmail(socket.data.email))) {
        await removeDraft(id)
        socket.send(ClientCmds.goToCreate)
    } else console.log("User " + socket.data.email + " does not own draft " + id + ", but tried to delete that draft!!")
}