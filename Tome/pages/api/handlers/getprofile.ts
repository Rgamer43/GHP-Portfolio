import { Socket } from 'socket.io';
import { getProfileByUsername, getUsernameByEmail } from '../database/users';

export async function getProfile(user: string, socket: Socket) {
    if(user === undefined || user == "default" || user === null) user = await getUsernameByEmail(socket.data.email)
    let profile = await getProfileByUsername(user)
    if(profile!.time !== null && profile!.time !== undefined) {
        profile!.time = new Date(profile!.time).toLocaleDateString()
    } else profile!.time = "before the dawn of time"
    socket.emit("profile", profile)
}