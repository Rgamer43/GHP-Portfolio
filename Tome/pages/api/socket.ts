import { Server } from 'socket.io';
import { Socket } from 'socket.io';
import message from './handlers/message';
import signIn from './handlers/signin';
import signOut from './handlers/signout';
import createAccount from './handlers/createaccount';
import { getProfile } from './handlers/getprofile';
import { getDefaultProfile } from './handlers/getdefaultprofile';
import { createDraft } from './handlers/createdraft';
import { getDrafts } from './handlers/getdrafts';
import { ObjectId } from 'mongodb';
import { getDraft } from '../api/handlers/getdraft';
import { updateDraft } from './handlers/updatedraft';
import { publishDraft } from './handlers/publishdraft';
import { deleteDraft } from './handlers/deletedraft';
import { getPost } from './handlers/getpost';
import { getProfilePosts } from './handlers/getprofileposts';
import { likePost } from './handlers/likepost';
import { dislikePost } from './handlers/dislikepost';
import { postComment } from './handlers/postcomment';
import { getIDByEmail } from './database/users';
import { discover } from './handlers/discover';
import { deletePost } from './handlers/deletepost';
import { deleteComment } from './handlers/deletecomment';

console.log("Initializing server...")

const SocketHandler = (req: any, res: any) => {
    if(res.socket.server.io) {
        console.log("Socket already set up")
        res.end()
        return
    }

    console.log("New connection!")

    const io = new Server(res.socket.server)
    res.socket.server.io = io

    io.on("connection", (socket: Socket) => {
        socket.on("message", (msg) => {
            message(msg, socket)
        })

        socket.on("signin", (email: string) => {
            console.log("Received signin event...")
            signIn(email, socket)
        })

        socket.on("signout", (email: string[]) => {
            console.log("Received signout event for " + socket.data.email)
            signOut(email[0])
        })

        socket.on("createaccount", (args: string[]) => {
            console.log("Received createaccount event...")
            createAccount(socket, args)
        })

        socket.on("getprofile", (user: string) => {
            console.log("Received getprofile event for " + user)
            getProfile(user, socket)
        })

        socket.on("getdefaultprofile", () => {
            console.log("Received getdefaultprofile event for " + socket.data.email)
            getDefaultProfile(socket)
        })

        socket.on("createdraft", () => {
            console.log("Received createdraft draft event for " + socket.data.email);
            if(socket.data.email !== undefined)
                createDraft(socket)
            else socket.emit("seterror", "You must be signed in to do that.")
        })

        socket.on("getdrafts", () => {
            console.log("Received getdrafts event for " + socket.data.email)
            if(socket.data.email !== undefined)
                getDrafts(socket)
            else socket.emit("seterror", "You must be signed in to do that.")
        })

        socket.on("getdraft", (id: string) => {
            console.log("Received getdraft event for " + socket.data.email + ", Draft ID: " + ObjectId.createFromHexString(id))
            if(socket.data.email !== undefined)
                getDraft(socket, ObjectId.createFromHexString(id))
            else socket.emit("seterror", "You must be signed in to do that.")
        })

        socket.on("updatedraft", (draft: any) => {
            console.log("Received updatedraft event for " + socket.data.email)
            if(socket.data.email !== undefined)
                updateDraft(socket, draft)
            else socket.emit("seterror", "You must be signed in to do that.")
        })

        socket.on("publishdraft", (draft: any) => {
            console.log("Received publishdraft event for " + socket.data.email)
            if(socket.data.email !== undefined)
                publishDraft(socket, draft)
            else socket.emit("seterror", "You must be signed in to do that.")
        })

        socket.on("deletedraft", (id: string) => {
            console.log("Received deletedraft event for " + socket.data.email)
            if(socket.data.email !== undefined)
                deleteDraft(socket, ObjectId.createFromHexString(id))
            else socket.emit("seterror", "You must be signed in to do that.")
        })

        socket.on("getpost", (id: string) => {
            console.log("Received getpost event for " + socket.data.email)
            getPost(socket, ObjectId.createFromHexString(id))
        })

        socket.on("getprofileposts", (user: string) => {
            console.log("Received getprofileposts event for " + socket.data.email + ", target user: " + user)
            getProfilePosts(socket, user)
        })

        socket.on("likepost", (post: string) => {
            console.log("Received likepost event for " + socket.data.email + ", target post: " + post)
            if(socket.data.email !== undefined)
                likePost(socket, post)
            else socket.emit("seterror", "You must be signed in to do that.")
        })

        socket.on("dislikepost", (post: string) => {
            console.log("Received dislikepost event for " + socket.data.email + ", target post: " + post)
            if(socket.data.email !== undefined)
                dislikePost(socket, post)
            else socket.emit("seterror", "You must be signed in to do that.")
        })

        socket.on("postcomment", async (args: string[]) => {
            console.log("Received postcomment event for " + socket.data.email + ", target post: " + args[0] + ", comment: " + args[1])
            if(socket.data.email !== undefined)
                postComment(socket, ObjectId.createFromHexString(args[0]), args[1], await getIDByEmail(socket.data.email))
            else socket.emit("seterror", "You must be signed in to do that.")
        })

        socket.on("discover", (mode: string) => {
            console.log("Received discover event for " + socket.data.email + ", mode: " + mode)
            discover(socket, mode)
        })

        socket.on("deletepost", async (post: string) => {
            console.log("Received deletepost event for " + socket.data.email + ", target post: " + post)
            if(socket.data.email !== undefined)
                deletePost(socket, ObjectId.createFromHexString(post))
            else socket.emit("seterror", "You must be signed in to do that.")
        })

        socket.on("deletecomment", async (args: any[]) => {
            console.log("Received deletecomment event for " + socket.data.email + ", target post: " + args[0] + ", target comment index: " + args[1])
            if(socket.data.email !== undefined)
                deleteComment(socket, ObjectId.createFromHexString(args[0]), args[1])
            else socket.emit("seterror", "You must be signed in to do that.")
        })
    })

    res.end()
}

console.log("Server initialized!")

export default SocketHandler
