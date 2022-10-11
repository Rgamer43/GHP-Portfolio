import { Db, MongoClient, ObjectId } from "mongodb"
import { getDB } from "./util"

export async function includesVerifiedEmail(email: string): Promise<boolean> {
    try {
        const db = getDB()
        const result = await db.collection('verifiedEmails').find({
            email: email
        })

        return (result !== null && result !== undefined)
    } catch {

    }

    return false
}

export async function addVerifiedEmail(email: string) {
    const db = getDB()
    if(!await includesVerifiedEmail(email)) {
            db.collection("verifiedEmails").insertOne({
            email: email
        })
    }
}

export async function removeVerifiedEmail(email: string) {
    const db = getDB()
    db.collection('verifiedEmails').deleteOne({email: email})
}