import { getSession, signIn, signOut, useSession } from 'next-auth/react';
import { Socket } from 'socket.io-client';
import { io } from 'socket.io-client';
import { Draft } from '../components/draft';
import { useSessionData } from '../components/signin';
import { ClientCmds, Session, ServerCmds } from '../pages/api/types';
import { ObjectId } from 'mongodb';
import Router, { useRouter } from 'next/router';

const productionURL: string = "", devURL: string = "http://localhost:3000"

function getURL(): string {
    if(process.env.DEV) return devURL
    else return productionURL
}

export const apiPath: string = getURL() + "/api/socket"

enum BootStatus {
    unbooted,
    booting,
    booted
}

let onBoot: {(): any}[] = []

let initStatus: BootStatus = BootStatus.unbooted, signInVerified: BootStatus = BootStatus.unbooted
export let shouldCreateAccount = false

export function handleSignOut() {
    console.log("Attempting sign out...")
    if(useSessionData.status == "authenticated") {
        getSession().then((result) => {
            socket.emit("signout", result?.user?.email)
            signOut()
        })
    }
}

const finishInit = async (s: SocketWrapper) => {
    console.log("Continuing with tag handlers...")

    s.on('goToProfile', (user: string) => {
        Router.push({
            pathname: 'profile',
            query: {
                user: user
            }
        })
    })

    s.on('profile', (result: any) => {
        console.log("Got profile:")
        console.log(result)

        if(result !== null) {
            if(result.username !== null && document.getElementById('username') !== null && document.getElementById('username')!.innerHTML !== null)
                document.getElementById('username')!.innerHTML = result.username
            if(result.bio !== null && document.getElementById('bio') !== null && document.getElementById('bio')!.innerHTML !== null) 
                document.getElementById('bio')!.innerHTML = result.bio
            if(result.username !== null && document.getElementById('pageTitle') !== null && document.getElementById('pageTitle')!.innerHTML !== null) 
                document.getElementById('pageTitle')!.innerHTML = result.username + " | Tome"
            if(document.getElementById('time')!.innerHTML !== null) {
                document.getElementById('time')!.innerHTML = "Member since " + result.time
                if(result.isAdmin) document.getElementById('time')!.innerHTML += ", Admin"
            }
        } else onDefaultProfile()
    })

    s.on('setdrafts', (drafts: any[], posts: any[]) => {
        console.log("Got drafts: ")
        console.log(drafts)
        
        if(drafts !== null) {
            Router.push({
                pathname: 'create',
                query: {
                    drafts: JSON.stringify(drafts),
                    posts: JSON.stringify(posts)
                }
            })
        }
    })

    s.on('setdraft', (result: any) => {
        console.log("Got draft: ")
        console.log(result)
        
        if(result !== null) {
            if(result.title !== null && document.getElementById('title') !== null) (document.getElementById('title') as HTMLInputElement).value = result.title
            if(result.thumbnail !== null && document.getElementById('thumbnail') !== null) (document.getElementById('thumbnail') as HTMLInputElement).value = result.thumbnail
            if(result.headerImage !== null && document.getElementById('headerImage') !== null) (document.getElementById('headerImage') as HTMLInputElement).value = result.headerImage
            if(result.body !== null && document.getElementById('body') !== null) (document.getElementById('body') as HTMLInputElement).value = result.body
        }
    })

    s.on('gotodraft', (result: any) => {
        goToDraft(result)
    })

    s.on('wordcounterror', (result: any[]) => {
        console.log("Word count too low.")
        console.log("Word Count: " + result[0] + ", Word Min: " + result[1])
        
        if(document.getElementById('error') !== null)
            document.getElementById('error')!.innerHTML = "You must have at least " + result[1] + " words and less than " + result[2] + " words to publish. You currently have " + 
                result[0] + " words (" + (result[1]-result[0]) + " more words needed)."
    })

    s.on('gotopost', (result: ObjectId) => {
        goToPost(result)
    })

    s.on('setpostdata', async (result: any) => {
        console.log("Setting post data. Data:")
        console.log(result)
        await Router.push({
            pathname: "post",
            query: {
                data: JSON.stringify(result)
            }
        })
    })

    s.on('setprofileposts', (result: any[]) => {
        console.log("Got profile posts")
        console.log(result)
        Router.push({
            pathname: "profile",
            query: {
                user: Router.query.user,
                posts: JSON.stringify(result)
            }
        })
    })

    s.on('seterror', (error: string) => {
        if(document.getElementById('error') !== null)
            document.getElementById('error')!.innerHTML = error
    })

    s.on('setbody', (content: string) => {
        console.log("Setting post body...")
        if(document.getElementById('body') !== undefined)
            document.getElementById('body')!.innerHTML = content
    })

    s.on('discover', async (data: string[]) => {
        console.log("Received discover data")
        console.log(data)

        Router.push({
            pathname: "discover",
            query: {
                mode: data[0],
                posts: JSON.stringify(data[1])
            }
        })
    })

    s.on('setadmin', (admin: boolean) => {
        localStorage.setItem("isAdmin", String(admin))
    })

    console.log("Initted socket")
}

const socketInitializer = async () => {
    if(initStatus == BootStatus.unbooted) {
        initStatus = BootStatus.booting
        console.log("Initting socket...")

        await fetch(apiPath)
        socket.socket = io()

        socket.socket.on('connect', () => {
            console.log('Connected! Continuing with part 2 of initting...')
        })

        socket.socket.on('message', (msg: any) => {
            if(msg == ClientCmds.forceSignOut) {
                console.log("Forcing sign out...")
                if(useSessionData.status == "authenticated")
                    handleSignOut()
                else console.log("Not signed in")
            } else if(msg == ClientCmds.createAccount) {
                console.log("Redirecting to account creation...")
                let u: string[] = window.location.href.split('/')
                if(!u[u.length-1].includes('signup'))
                    window.location.href = 'signup'
            } else if(msg == ClientCmds.usernameTaken) {
                if(document !== null)
                    document.getElementById("error")!.innerHTML = "Username taken."
            } else if(msg == ClientCmds.goToCreate) {
                Router.push({
                    pathname: "create"
                })
            } else if(msg == ClientCmds.reloadPost) {
                Router.push({
                    pathname: Router.pathname,
                    query: {
                        id: JSON.parse(Router.query.data as string)._id
                    }
                })
            }
        })

        initStatus = BootStatus.booted
        console.log("Added message handler. InitStatus is booted. Adding tag handlers...")

        finishInit(socket)

        console.log("Running delayed network functions... Count: " + onBoot.length)
        onBoot.forEach(async element => {
            console.log("Running delayed network function... Session Defined: " + (await getSession()!==null && await getSession()!==undefined))
            element()
        });
        console.log("Finished running delayed network functions")
    }
}

class SocketWrapper {
    socket: Socket | undefined;
    emit: (tag: string, ...args: any) => any;
    send: (...args: ServerCmds[]) => any;
    on: (eventName: string, ...listener: any) => any;
    once: (eventName: string, ...listener: any) => any;

    constructor() {
        this.emit = async (tag: string, args: any) => {
            if(initStatus != BootStatus.booting) {
                if(this.socket === null || this.socket === undefined) await init()
                if(this.socket !== null && this.socket !== undefined) this.socket.emit(tag, args)
                else console.error("Socket is still null and undefined! Socket Status: " + BootStatus[initStatus])
            } else onBoot.push(() => {
                this.socket?.emit(tag, args)
            })
        }

        this.send = async (args: any) => {
            if(initStatus != BootStatus.booting) {
                if(this.socket === null || this.socket === undefined) await init()
                if(this.socket !== null && this.socket !== undefined) this.socket.send(args)
                else console.error("Socket is still null and undefined! Socket Status: " + BootStatus[initStatus])
            } else onBoot.push(() => {
                this.socket?.send(args)
            })
        }

        this.on = async (eventName: string, listener: any) => {
            if(initStatus != BootStatus.booting) {
                if(this.socket === null || this.socket === undefined) await init()
                if(this.socket !== null && this.socket !== undefined) this.socket.on(eventName, listener)
                else console.error("Socket is still null and undefined! Socket Status: " + BootStatus[initStatus])
            } else onBoot.push(() => {
                this.socket?.on(eventName, listener)
            })
        }

        this.once = async (eventName: string, listener: any) => {
            if(initStatus != BootStatus.booting) {
                if(this.socket === null || this.socket === undefined) await init()
                if(this.socket !== null && this.socket !== undefined) this.socket.once(eventName, listener)
                else console.error("Socket is still null and undefined! Socket Status: " + BootStatus[initStatus])
            } else onBoot.push(() => {
                this.socket?.once(eventName, listener)
            })
        }
    }
}

let signedIn: boolean = false
export var socket: SocketWrapper = new SocketWrapper()

export function getSignedIn(): boolean {
    return signedIn;
}

export function setSignedIn(newValue: boolean): boolean {
    signedIn = newValue;
    return signedIn;
}

export async function testPing() {
    console.log("Test pinging...")
    socket.send(ServerCmds.testPing)
    console.log(await getSession())
}

export function handleSignIn(e: any) {
    console.log("Signing in...")
    e.preventDefault()  
    signIn("google")
    verifySignIn()
}

export function handleSignInNoEvent() {
    console.log("Signing in...")
    signIn("google")
    verifySignIn()
}

export async function verifySignIn() {
    if(signInVerified == BootStatus.unbooted) {
        signInVerified = BootStatus.booting
        console.log("Verifying sign in...")
        getSession().then((result) => {
            console.log("Got session")
            result = result as Session

            if(result !== undefined && result !== null) {
                if(result.user !== undefined) {
                    socket.emit("signin", result.user.email)
                    signInVerified = BootStatus.booted
                    console.log("Sign in verified")
                } else {
                    console.log("User is null or undefined")
                    signInVerified = BootStatus.unbooted
                }
            } else {
                console.log("Result is null or undefined")
                signInVerified = BootStatus.unbooted
            }
        })
        
    }
}

export async function init() {
    if(initStatus == BootStatus.unbooted) {
        console.log("Awaiting socketInitiliazer...")
        await socketInitializer()
        console.log("Done awaiting socketInitializer")
    }
}

export function createAccount(username: string, bio: string) {
    socket.emit("createaccount", [username, bio])
}

export function getProfile(user: string) {
    console.log("Retrieving profile for " + user)
    if(user !== null && user !== undefined) socket.emit("getprofile", user)
    else onDefaultProfile()
}

export function onDefaultProfile() {
    console.log("Retrieving default profile...")
    socket.emit("getdefaultprofile")
}

export function createNewDraft() {
    console.log("Creating new draft...")
    socket.emit("createdraft")
}

export function getDrafts() {
    console.log("Getting drafts...")
    socket.emit("getdrafts")
}

export function getDraft() {
    console.log("Getting draft...")
    socket.emit("getdraft", Router.query.draftID)
}

export function updateDraft(draft: any) {
    socket.emit("updatedraft", draft)
}

export function goToDraft(id: ObjectId) {
    Router.push({
        pathname: "write",
        query: {
            draftID: id.toString()
        }
    })
}

export function publishDraft(draft: any) {
    console.log("Publishing draft...")
    socket.emit("publishdraft", draft)
}

export function deleteDraft(id: string) {
    console.log("Deleting draft...")
    socket.emit("deletedraft", id)
}

export function goToPost(id: ObjectId) {
    console.log("Going to post: " + id)
    Router.push({
        pathname: "post",
        query: {
            id: id.toString()
        }
    })
}

export function getPost(id: string) {
    console.log("Getting post " + id)
    socket.emit("getpost", id)
}

export function getProfilePosts(user: string) {
    console.log("Getting posts for " + user)
    socket.emit("getprofileposts", user)
}

export function likePost(post: string) {
    console.log("Liking post " + post)
    socket.emit("likepost", post)
}

export function dislikePost(post: string) {
    console.log("Disliking post " + post)
    socket.emit("dislikepost", post)
}

export function postComment(comment: string) {
    let id = JSON.parse(Router.query.data as string)._id
    console.log("Posting comment: " + comment)
    socket.emit("postcomment", [id, comment])
}

export function discoverPosts(mode: string) {
    console.log("Discovering posts... Mode: " + mode)
    socket.emit("discover", mode)
}

export function isAdmin(): boolean {
    if(typeof window !== undefined)
        if(localStorage !== undefined) return (localStorage.getItem("isAdmin") === "true")
        else return false
    else return false
}