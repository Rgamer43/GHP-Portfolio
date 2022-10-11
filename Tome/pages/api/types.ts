import { JWT } from "next-auth/jwt";
import { Socket } from "socket.io";

export enum ClientCmds {
    forceSignOut,
    createAccount,
    usernameTaken,
    goToCreate,
    reloadPost
}

export enum ServerCmds {
    createDraft,
    getDefaultProfile,
    testPing
}

export type Session = {
    user: {
        email: string,
        name: string,
        image: string
    },
    expires: string
}