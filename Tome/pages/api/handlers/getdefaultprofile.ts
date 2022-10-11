import { Socket } from 'socket.io';
import { getProfileByEmail, getProfileByUsername, getUsernameByEmail } from '../database/users';
import { ClientCmds } from '../types';

export async function getDefaultProfile(socket: Socket) {
    console.log("Getting default profile...")
    socket.emit("goToProfile", await getUsernameByEmail(socket.data.email))
}