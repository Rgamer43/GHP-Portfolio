import { NextPage } from "next"
import Router, { useRouter } from "next/router";
import { useEffect } from 'react';
import { PostCard } from "../components/postcard";
import { discoverPosts } from "../lib/networkmanager";

const Discover: NextPage = () => {
    let mode: string = "Explore", posts: any[] = []
    if(useRouter().query.posts !== undefined) posts = JSON.parse(useRouter().query.posts as string)

    useEffect(() => {
        mode = Router.query.mode as string

        if(Router.query.posts === undefined) {
            discoverPosts(mode)
        }

        if(mode === undefined || mode === "") mode = "explore"

        mode = mode.charAt(0).toUpperCase() + mode.slice(1, mode.length)
        document.getElementById("mode")!.innerHTML = mode
    })

    return (
        <>
            <div className="m-5">
                <h1 className="text-3xl" id="mode">{mode}</h1>
                {posts.map((value, index) => {return <PostCard title={value.title} body={value.body} thumbnail={value.thumbnail} 
                    id={value._id} views={value.views} likes={value.likes} dislikes={value.dislikes} author={value.owner}></PostCard>})}
            </div>
        </>
    )
}

export default Discover