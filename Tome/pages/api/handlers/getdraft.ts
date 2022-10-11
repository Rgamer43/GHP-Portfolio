import { Socket } from 'socket.io';
import { ObjectId } from 'mongodb';
import { getDraftByID } from '../database/drafts';

export async function getDraft(socket: Socket, id: ObjectId) {
    socket.emit("setdraft", await getDraftByID(id))
}