import { NextPage } from "next"
import { useRouter } from 'next/router';
import Image from "next/image";
import { Anchor } from "../components/anchor";
import { dislikePost, getPost, likePost, postComment, isAdmin, socket } from '../lib/networkmanager';
import { Button } from '../components/button';
import { useEffect } from 'react';
import Router from 'next/router';
import { Comment } from'../components/comment'

const submit = (event: any) => {
    event.preventDefault()
    if(event.target.comment.value !== "") {
        postComment(event.target.comment.value)
        event.target.comment.value = ""
    }

}

const deleteThisPost = (id: string) => {
    if(confirm("Are you sure you want to delete this post?") === true) socket.emit("deletepost", id)
}

const Post: NextPage = () => {
    let id: string, title: string = "", author: string = "", body: string = "", headerImage: string = "", views: number = 0, 
        likes: number = 0, dislikes: number = 0, hasLiked: boolean = false, hasDisliked: boolean = false, comments: any[] = [],
        time: string = "", headerWidth = 1920, admin = false

    let data: any = useRouter().query.data
    if(data !== undefined) {
        data = JSON.parse(Router.query.data as string)
        console.log("Got post data:")
        console.log(data)
        id = data._id
        title = data.title
        author = data.owner
        body = data.body
        headerImage = "/api/imageproxy?url=" + data.headerImage
        views = data.views
        likes = data.likes
        dislikes = data.dislikes
        hasLiked = data.hasLiked
        hasDisliked = data.hasDisliked
        comments = data.comments
        time = data.time
        headerWidth = window.innerWidth
    }

    useEffect(() => {
        if(Router.query.id !== undefined) {
            id = Router.query.id as string
            getPost(id as string)
        } else {
            if(Router.query.data !== undefined) {
                admin = isAdmin()
            }
        }
    })
    

    return (
        <>
            <div className="bg-lighterSubtle">
                <Image id="thumbnail" className="object-cover" src={headerImage} width={headerWidth} height={250}/>
                <div className="ml-48 mr-48 bg-lightest p-4">
                    <h1 id="title" className="text-4xl">{title}</h1>
                    <h2 className="text-2xl mb-4">By 
                        <Anchor href={"/profile"} query={{user:author}}><span id="author">{" " + author}</span></Anchor>,
                        {" " + time} {admin && <Button onClick={()=>{deleteThisPost(id as string)}}>Delete this post</Button>}
                    </h2>
                    <p className="mb-6">Views: {views}, Likes: {likes}, Dislikes: {dislikes} 
                        {(!isNaN(likes/(dislikes+likes))) && " (" + Math.round((likes/(dislikes+likes))*100) + "%)"}</p>
                    <p id="body" className="mb-6">
                        {body}
                    </p>
                    <div className="flex justify-evenly">
                        <Button onClick={() => {likePost(id as string)}} className="text-2xl">{(hasLiked) ? "Unlike" : "Like"}</Button>
                        <Button onClick={() => {dislikePost(id as string)}} className="text-2xl">{(hasDisliked) ? "Undislike" : "Dislike"}</Button>
                    </div>
                    <div>
                        <form action="" className="m-4" onSubmit={submit}>
                            <label htmlFor="comment" className="text-xl ml-2">Comment</label>
                            <textarea name="comment" id="comment" className="bg-lightest border-medium focus:border-dark border-2 rounded-md pb-4 pl-1 w-full h-28"></textarea>
                            <Button type='submit' className={"w-10 text-xl ml-2"}>Post</Button>
                            <h3 id="error" className="ml-1 text-red-600"></h3>
                        </form>
                    </div>
                    <div>
                        {comments.reverse().map((value, index) => {
                            return <Comment user={value.user} text={value.text} time={value.time} post={id as string} id={comments.length - index - 1} admin={admin}></Comment>
                        })}
                    </div>
                </div>
            </div>
        </>
    )
}

export default Post