import { Db, MongoClient } from "mongodb"

const client = new MongoClient(process.env.MONGO_URI!)

export function getDB(): Db {
    return client.db("storage")
}

export async function listDatabases() {
    let databases = await client.db().admin().listDatabases()
    console.log("Databases:")
    databases.databases.forEach(db => console.log("-" + db.name))
}

export const seconds: number = 1000
export const minutes: number = seconds * 60
export const hours: number = minutes * 60
export const days: number = hours * 24
export const weeks: number = days * 7