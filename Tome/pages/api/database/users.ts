import { getDB } from './util';
import { ObjectId } from 'mongodb';

function getCollection() {
    return getDB().collection('users')
}

export async function includesUser(email: string): Promise<boolean> {
    const db = getDB()
    const result = await db.collection("users").findOne({email: email})
    return (result !== undefined && result !== null)
}

export async function includesUserByUsername(username: string): Promise<boolean> {
    const db = getDB()
    const result = await db.collection("users").findOne({username: username})
    return (result !== undefined && result !== null)
}

export async function addUser(email: string, username: string, bio?: string) {
    console.log("Attempting to add user...")
    if(!await includesUser(email)) {
        console.log("User does not already exist")
            getCollection().insertOne({
            email: email,
            username: username,
            bio: bio,
            drafts: [],
            posts: [],
            time: Date.now(),
            perms: "default"
        })
        console.log("Added user " + username + " (" + email + ")")
    } else console.log("User already exists")
}

export async function getProfileByUsername(username: string) {
    const result = await getCollection().findOne({username: username})
    
    if(result === null) return null
    else {
        return {
            username: result.username,
            bio: result.bio,
            time: result.time,
            isAdmin: result.isAdmin
        }
    }
}

export async function getProfileByEmail(email: string) {
    const result = await getCollection().findOne({email: email})
    
    if(result === null) return null
    else {
        return {
            username: result.username,
            bio: result.bio
        }
    }
}

export async function getUsernameByEmail(email: string): Promise<string> {
    const result = await getCollection().findOne({email: email})
    
    if(result === null) return "null"
    else {
        return result.username
    }
}

export async function getIDByEmail(email: string): Promise<ObjectId> {
    const result = await getCollection().findOne({email: email})
    
    if(result === null) {
        console.log("Could not find ID by email! Email: " + email)
        return new ObjectId()
    }
    else {
        return result._id
    }
}

export async function getDraftIDsByUserID(id: ObjectId): Promise<any[]> {
    const result = await getCollection().findOne({_id: id})
    
    if(result === null) return []
    else {
        return result.drafts
    }
}

export async function getPostIDsByUserID(id: ObjectId): Promise<any[]> {
    const result = await getCollection().findOne({_id: id})
    
    if(result === null) return []
    else {
        return result.posts
    }
}

export async function getUsernameByID(id: ObjectId) {
    return (await getCollection().findOne({_id: id}))!.username
}

export async function getIDByUsername(user: string) {
    return (await getCollection().findOne({username: user}))!._id
}

export async function getIsAdmin(id: ObjectId): Promise<boolean> {
    return (await getCollection().findOne({_id: id}))!.perms === "admin"
}

export async function getIsAdminByEmail(email: string): Promise<boolean> {
    return await getIsAdmin(await getIDByEmail(email))
}