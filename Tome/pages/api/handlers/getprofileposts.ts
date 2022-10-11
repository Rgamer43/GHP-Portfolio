import { Socket } from "socket.io";
import { getDislikes, getLikes, getPostsByIDs, getViews } from "../database/posts";
import { getIDByUsername, getPostIDsByUserID, getUsernameByID } from "../database/users";

export async function getProfilePosts(socket: Socket, user: string) {
    let posts: any[] = await getPostsByIDs(await getPostIDsByUserID(await getIDByUsername(user)))

    for (let i = 0; i < posts.length; i++) {
        const element = posts[i];
        element.owner = await getUsernameByID(element!.owner)
        // element.views = await getViews(element._id)
        // element.likes = await getLikes(element._id)
        // element.dislikes = await getDislikes(element._id)
        element.body = element.body.slice(0, process.env.CARD_BODY_MAX as unknown as number)
    }

    socket.emit("setprofileposts", posts)
}