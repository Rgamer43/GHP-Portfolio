import { Socket } from 'socket.io';
import { getDraftsByIDs } from '../database/drafts';
import { getDislikes, getLikes, getPostsByIDs, getViews } from '../database/posts';
import { getDraftIDsByUserID, getIDByEmail, getPostIDsByUserID, getUsernameByID } from '../database/users';

export async function getDrafts(socket: Socket) {
    let drafts: any[] = await getDraftsByIDs(await getDraftIDsByUserID(await getIDByEmail(socket.data.email)))
    let posts: any[] = await getPostsByIDs(await getPostIDsByUserID(await getIDByEmail(socket.data.email)))

    for (let i = 0; i < drafts.length; i++) {
        const element = drafts[i];
        element.body = element.body.slice(0, process.env.CARD_BODY_MAX as unknown as number)
    }
    
    for (let i = 0; i < posts.length; i++) {
        const element = posts[i];
        element.owner = await getUsernameByID(element!.owner)
        // element.views = await getViews(element._id)
        // element.likes = await getLikes(element._id)
        // element.dislikes = await getDislikes(element._id)
        element.body = element.body.slice(0, process.env.CARD_BODY_MAX as unknown as number)
    }

    socket.emit("setdrafts", drafts, posts)
}