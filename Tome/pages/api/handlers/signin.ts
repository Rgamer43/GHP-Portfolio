import { Session } from "../types"
import { Socket } from "socket.io"
import socket from "../socket"
import { ClientCmds } from '../types';
import { deflateSync } from "zlib";
import { time } from "console";
import { includesVerifiedEmail } from "../database/verifiedEmails";
import { getIDByEmail, getIsAdmin, includesUser } from "../database/users";

let e: string, s: Socket, interval: NodeJS.Timer

export default function signIn (email: string, socket: Socket) {
    console.log("Verifying sign in on server...")
    e = email
    s = socket
    interval = setInterval(partTwo, 250)
}

async function partTwo () {
    clearInterval(interval)

    if(e !== undefined && e !== null) {
        if(await includesVerifiedEmail(e)) {
            s.data.email = e
            console.log("Email verified. Email: " + e)
            onVerify()
        } else {
            console.log("Unverified sign in attempted!")
            s.send(ClientCmds.forceSignOut)
            s.disconnect()
            console.log("Socket should be disconnected. Socket Connection Status: " + s.connected)
        }
    } else console.log("Session is null or undefined!")
}

async function onVerify() {
    if(!await includesUser(e)) {
        console.log("Client needs to create an account")
        s.send(ClientCmds.createAccount)
    } else {
        console.log("Client account exists")
        s.emit("setadmin", await getIsAdmin(await getIDByEmail(e)))
    }
}