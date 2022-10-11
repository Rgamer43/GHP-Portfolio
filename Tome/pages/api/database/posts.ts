import { getDB, days, weeks } from './util';
import { FindCursor, ObjectId, WithId, Document } from 'mongodb';
import { getPopularity, getTop, getHot } from './algorithms';

function getCollection() {
    return getDB().collection('posts')
}

function getUsers() {
    return getDB().collection('users')
}

function getDrafts() {
    return getDB().collection('drafts')
}

export async function getPostByID(id: ObjectId) {
    let post = await getCollection().findOne({_id: id})

    if(post!.comments === undefined) {
        await getCollection().updateOne({_id: post}, {
            $set: {
                comments: []
            }
        })
        post = await getCollection().findOne({_id: id})
    }

    return post
}

export async function addPost(draft: any, owner: ObjectId) {
    const id = await getCollection().insertOne({
        title: draft.title,
        thumbnail: draft.thumbnail,
        body: draft.body,
        owner: owner,
        headerImage: draft.headerImage,
        time: Date.now(),
        comments: []
    })

    await getUsers().updateOne({_id: owner}, {
        $push: {
            posts: id.insertedId
        },
        $pull: {
            drafts: draft._id
        }
    })

    await getDrafts().deleteOne({_id: draft._id})

    return id.insertedId
}

export async function getPostsByIDs(ids: ObjectId[]) {
    let posts: any[] = []

    for (let i = 0; i < ids.length; i++) {
        const element = ids[i];
        posts.push(await getPostByID(element))
    }
    
    return posts
}

export async function addView(id: ObjectId, user: ObjectId): Promise<number> {
    if((await getPostByID(id))!.views === undefined) {
        await getCollection().updateOne({_id: id}, {
            $set: {
                views: []
            }
        })
    }

    let includes = false, views = (await getPostByID(id))!.views
    for (let i = 0; i < views.length; i++) {
        const element = views[i];
        if(element.equals(user)) includes = true
    }

    if(!includes)
        await getCollection().updateOne({_id: id}, {
            $push: {
                views: user
            }
        })

    return (await getCollection().findOne({_id: id}))!.views.length
}

export async function getViews(id: ObjectId): Promise<number> {
    if((await getPostByID(id))!.views === undefined) {
        await getCollection().updateOne({_id: id}, {
            $set: {
                views: []
            }
        })
    }

    return (await getCollection().findOne({_id: id}))!.views.length
}

export async function addLike(id: ObjectId, user: ObjectId): Promise<number> {
    if((await getPostByID(id))!.likes === undefined) {
        await getCollection().updateOne({_id: id}, {
            $set: {
                likes: []
            }
        })
    }

    if(await hasDisliked(id, user)) removeDislike(id, user)

    let includes = false, likes = (await getPostByID(id))!.likes
    for (let i = 0; i < likes.length; i++) {
        const element = likes[i];
        if(element.equals(user)) includes = true
    }

    if(!includes)
        await getCollection().updateOne({_id: id}, {
            $push: {
                likes: user
            }
        })

    return (await getCollection().findOne({_id: id}))!.likes.length
}

export async function removeLike(id: ObjectId, user: ObjectId): Promise<number> {
    if((await getPostByID(id))!.likes === undefined) {
        await getCollection().updateOne({_id: id}, {
            $set: {
                likes: []
            }
        })
    }

    let includes = false, likes = (await getPostByID(id))!.likes
    for (let i = 0; i < likes.length; i++) {
        const element = likes[i];
        if(element.equals(user)) includes = true
    }

    if(includes)
        await getCollection().updateOne({_id: id}, {
            $pull: {
                likes: user
            }
        })

    return (await getCollection().findOne({_id: id}))!.likes.length
}

export async function getLikes(id: ObjectId): Promise<number> {
    if((await getPostByID(id))!.likes === undefined) {
        await getCollection().updateOne({_id: id}, {
            $set: {
                likes: []
            }
        })
    }

    return (await getCollection().findOne({_id: id}))!.likes.length
}

export async function hasLiked(id: ObjectId, user: ObjectId): Promise<boolean> {
    if((await getPostByID(id))!.likes === undefined) {
        await getCollection().updateOne({_id: id}, {
            $set: {
                likes: []
            }
        })

        return false
    }

    let r = false
    await (await getCollection().findOne({_id: id}))!.likes.forEach((element: ObjectId) => {
        if((element as ObjectId).equals(user)) r = true
    });
    
    return r
}

export async function addDislike(id: ObjectId, user: ObjectId): Promise<number> {
    if((await getPostByID(id))!.dislikes === undefined) {
        await getCollection().updateOne({_id: id}, {
            $set: {
                dislikes: []
            }
        })
    }

    if(await hasLiked(id, user)) removeLike(id, user)

    let includes = false, dislikes = (await getPostByID(id))!.dislikes
    for (let i = 0; i < dislikes.length; i++) {
        const element = dislikes[i];
        if(element.equals(user)) includes = true
    }

    if(!includes)
        await getCollection().updateOne({_id: id}, {
            $push: {
                dislikes: user
            }
        })

    return (await getCollection().findOne({_id: id}))!.dislikes.length
}

export async function removeDislike(id: ObjectId, user: ObjectId): Promise<number> {
    if((await getPostByID(id))!.dislikes === undefined) {
        await getCollection().updateOne({_id: id}, {
            $set: {
                dislikes: []
            }
        })
    }

    let includes = false, dislikes = (await getPostByID(id))!.dislikes
    for (let i = 0; i < dislikes.length; i++) {
        const element = dislikes[i];
        if(element.equals(user)) includes = true
    }

    if(includes)
        await getCollection().updateOne({_id: id}, {
            $pull: {
                dislikes: user
            }
        })

    return (await getCollection().findOne({_id: id}))!.dislikes.length
}

export async function getDislikes(id: ObjectId): Promise<number> {
    if((await getPostByID(id))!.dislikes === undefined) {
        await getCollection().updateOne({_id: id}, {
            $set: {
                dislikes: []
            }
        })
    }

    return (await getCollection().findOne({_id: id}))!.dislikes.length
}

export async function hasDisliked(id: ObjectId, user: ObjectId): Promise<boolean> {
    if((await getPostByID(id))!.dislikes === undefined) {
        await getCollection().updateOne({_id: id}, {
            $set: {
                dislikes: []
            }
        })

        return false
    }

    let r = false
    await (await getCollection().findOne({_id: id}))!.dislikes.forEach((element: ObjectId) => {
        if((element as ObjectId).equals(user)) r = true
    })
    
    return r
}

export async function addComment(post: ObjectId, user: ObjectId, comment: string): Promise<boolean> {
    if((await getPostByID(post))!.comments === undefined) {
        await getCollection().updateOne({_id: post}, {
            $set: {
                comments: []
            }
        })
    }

    let i = false
    let comments = (await getPostByID(post))!.comments
    for (let j = 0; j < comments.length; j++) {
        const element = comments[j];
        if(element.text.toLowerCase() === comment.toLowerCase() && element.user.equals(user)) i = true       
    }

    if(!i) getCollection().updateOne({_id: post}, {
        $push: {
            comments: {
                user: user,
                text: comment,
                time: Date.now()
            }
        }
    })

    return !i
}

export async function getComments(post: ObjectId): Promise<{user: string, text: string}[]> {
    if((await getPostByID(post))!.comments === undefined) {
        await getCollection().updateOne({_id: post}, {
            $set: {
                comments: []
            }
        })
    }

    return (await getPostByID(post))!.comments
}

export async function findMoreRecentThanPosts(timeLimit: number): Promise<FindCursor<WithId<Document>>> {
    const minTime: number = Date.now()-timeLimit
    let found: FindCursor<WithId<Document>> = await getCollection().find({time: { $gt: minTime }})
    return found.sort({time: -1}).limit(process.env.SEARCH_LIMIT as unknown as number)
}

export async function getExplorePosts(): Promise<any[]> {
    let posts: any[] = await (await findMoreRecentThanPosts(weeks)).toArray()
    let sorted: any[] = []

    for (let i = 0; i < posts.length; i++) {
        const element = posts[i];
        await getPostByID(element._id)
        sorted.push({id: i, pop: getPopularity(element)})
    }

    sorted.sort((a, b) => {return b.pop - a.pop})    

    for (let i = 0; i < sorted.length; i++) {
        const score = sorted[i].pop
        sorted[i] = posts[sorted[i].id]
        sorted[i].score = score
    }

    return sorted
}

export async function getTopPosts(): Promise<any[]> {
    let posts: any[] = await (await findMoreRecentThanPosts(weeks * 2)).toArray()
    let sorted: any[] = []

    for (let i = 0; i < posts.length; i++) {
        const element = posts[i];
        await getPostByID(element._id)
        sorted.push({id: i, pop: getTop(element)})
    }

    sorted.sort((a, b) => {return b.pop - a.pop})

    for (let i = 0; i < sorted.length; i++) {
        const score = sorted[i].pop
        sorted[i] = posts[sorted[i].id]
        sorted[i].score = score
    }

    return sorted
}

export async function getHotPosts(): Promise<any[]> {
    let posts: any[] = await (await findMoreRecentThanPosts(days * 4)).toArray()
    let sorted: any[] = []

    for (let i = 0; i < posts.length; i++) {
        const element = posts[i];
        await getPostByID(element._id)
        sorted.push({id: i, pop: getHot(element)})
    }

    sorted.sort((a, b) => {return b.pop - a.pop})

    for (let i = 0; i < sorted.length; i++) {
        const score = sorted[i].pop
        sorted[i] = posts[sorted[i].id]
        sorted[i].score = score
    }

    return sorted
}

export async function getRecentPosts(): Promise<any[]> {
    let posts: any[] = await (await findMoreRecentThanPosts(weeks * 2)).toArray()

    for (let i = 0; i < posts.length; i++) {
        const element = posts[i];
        await getPostByID(element._id)
    }

    return posts
}

export async function deletePostByID(post: ObjectId) {
    getCollection().updateOne({_id: (await getCollection().findOne({_id: post}))!.owner}, {$pull: {posts: post}})
    getCollection().deleteOne({_id: post})
}

export async function deleteCommentByID(post: ObjectId, id: number) {
    getCollection().updateOne({_id: post}, 
        {$pull: 
            { 
                comments: (await getCollection().findOne({_id: post}))!.comments[id]
            }
        })
}