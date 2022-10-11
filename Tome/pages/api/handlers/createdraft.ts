import { Socket } from "socket.io";
import { addDraft } from "../database/drafts";
import { getIDByEmail } from "../database/users";

export async function createDraft(socket: Socket) {
    console.log("Creating new draft for " + socket.data.email)
    socket.emit("gotodraft", await addDraft(await getIDByEmail(socket.data.email)))
}