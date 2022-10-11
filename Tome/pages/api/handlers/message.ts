import { Socket } from "socket.io"
import socket from "../socket"
import { getDefaultProfile } from "./getdefaultprofile"
import { ServerCmds } from '../types';
import { createDraft } from "./createdraft";

export default function message (msg: ServerCmds, socket: Socket) {
    console.log("Message Event from " + socket.data.email + ": " + msg)
    
    if(msg == ServerCmds.getDefaultProfile) getDefaultProfile(socket)
    // else if(msg == ServerCmds.createDraft) createDraft(socket)
}