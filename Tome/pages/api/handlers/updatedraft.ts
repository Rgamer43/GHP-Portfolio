import { Socket } from "socket.io";
import { ObjectId } from 'mongodb';
import { getDraftIDsByUserID, getIDByEmail } from "../database/users";
import { getDraftByID, ownsDraft, updateDraftByID } from "../database/drafts";

export async function updateDraft(socket: Socket, draft: any) {
    draft._id = ObjectId.createFromHexString(draft._id)

    if(await ownsDraft(draft._id, await getIDByEmail(socket.data.email))) {
        await updateDraftByID(draft._id, draft)
    } else console.log("User does not own draft! User: " + socket.data.email + ", Draft ID: " + draft._id)
}