export function getPopularity(post: any): number {
    const views = process.env.POP_VIEW_WEIGHT as unknown as number,
        comments = process.env.POP_COMMENT_WEIGHT as unknown as number,
        likes = process.env.POP_LIKE_WEIGHT as unknown as number,
        dislikes = process.env.POP_DISLIKE_WEIGHT as unknown as number

    if(post.comments === undefined) post.comments = []
    
    let p: number = post.views.length * views + 
        post.comments.length * comments + 
        post.likes.length * likes -
        post.dislikes.length * dislikes

    return p
}

export function getTop(post: any): number {
    const views = process.env.TOP_VIEW_WEIGHT as unknown as number,
        comments = process.env.TOP_COMMENT_WEIGHT as unknown as number,
        likes = process.env.TOP_LIKE_WEIGHT as unknown as number,
        dislikes = process.env.TOP_DISLIKE_WEIGHT as unknown as number
    
    if(post.comments === undefined) post.comments = []
    
    let p: number = post.views.length * views + 
        post.comments.length * comments + 
        post.likes.length * likes -
        post.dislikes.length * dislikes

    return p
}

export function getHot(post: any): number {
    const views = process.env.HOT_VIEW_WEIGHT as unknown as number,
        comments = process.env.HOT_COMMENT_WEIGHT as unknown as number,
        likes = process.env.HOT_LIKE_WEIGHT as unknown as number,
        dislikes = process.env.HOT_DISLIKE_WEIGHT as unknown as number
    
    if(post.comments === undefined) post.comments = []
    
    let p: number = post.views.length * views + 
        post.comments.length * comments + 
        post.likes.length * likes -
        post.dislikes.length * dislikes

    return p
}