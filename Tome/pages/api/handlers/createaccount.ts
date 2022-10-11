import { Socket } from "socket.io";
import { addUser, includesUserByUsername, includesUser } from "../database/users";
import { ClientCmds } from '../types';

export default async function createAccount(socket: Socket, args: string[]) {
    console.log("Attempting to create account. Args:")
    console.log(args)
    const username: string = args[0]
    console.log("Socket Data:")
    console.log(socket.data)

    if(!await includesUser(socket.data.email) && socket.data.email !== undefined && socket.data.email !== null) {
        console.log("User does not yet have account")
        if(!await includesUserByUsername(username)) {
            console.log("Username is free")
            success(socket, username, args[1])
        } else {
            console.log("Username " + username + " is taken!")
            socket.send(ClientCmds.usernameTaken)
        }
    } else console.log("User already has account!")
}

function success(socket: Socket, username: string, bio: string) {
    console.log("Creating account...")
    addUser(socket.data.email, username, bio)
    socket.emit("goToProfile", username)
}