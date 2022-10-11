import { Collection, ObjectId, Document, WithId } from "mongodb";
import { getDraftIDsByUserID } from "./users";
import { getDB } from "./util";

function getCollection() {
    return getDB().collection('drafts')
}

function getUsers() {
    return getDB().collection('users')
}

export async function getDraftByID(id: ObjectId) {
    return await getCollection().findOne({_id: id})
}

export async function addDraft(id: ObjectId) {
    let c: Collection<Document> = getCollection()
    let draft = await getCollection().insertOne({
        owner: id,
        title: "Untitled Draft",
        body: "Enter your post's content here...",
        thumbnail: "",
        headerImage: ""
    })

    let drafts = await getDraftIDsByUserID(id)
    if(drafts !== undefined) drafts.push(draft.insertedId)
    else drafts = [draft.insertedId]

    await getUsers().updateOne({_id: id}, {
        $push: {
            drafts: draft.insertedId
        }
    })

    return draft.insertedId
}

export async function getDraftsByIDs(ids: ObjectId[]) {
    let drafts: any[] = []

    for (let i = 0; i < ids.length; i++) {
        const element = ids[i];
        drafts.push(await getDraftByID(element))
    }
    
    return drafts
}

export async function updateDraftByID(id: ObjectId, updated: any) {
    await getCollection().updateOne({_id: id}, {
        $set: {
            title: updated.title,
            thumbnail: updated.thumbnail,
            headerImage: updated.headerImage,
            body: updated.body
        }
    })
}

export async function ownsDraft(draft: ObjectId, claim: ObjectId): Promise<boolean> {
    let owner: ObjectId | undefined = (await getDraftByID(draft))!.owner
    console.log(owner)
    return owner!.equals(claim)
}

export async function removeDraft(id: ObjectId) {
    let owner: ObjectId | undefined = (await getDraftByID(id))!.owner
    getUsers().updateOne({_id: owner}, {
        $pull: {
            drafts: id
        }
    })
    getCollection().deleteOne({_id: id})
}