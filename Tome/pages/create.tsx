import { NextPage } from "next"
import { Button } from '../components/button';
import { useEffect } from 'react';
import { createNewDraft, getDrafts, handleSignIn, handleSignInNoEvent } from '../lib/networkmanager';
import Router, { useRouter } from 'next/router';
import { Draft } from '../components/draft';
import Head from "next/head";
import { ObjectId } from 'mongodb';
import { PostCard } from '../components/postcard';
import { useSession } from 'next-auth/react';

const Create: NextPage = () => {
    let drafts: any[] = []
    let posts: any[] = []
    if(useRouter().query.drafts !== undefined)
        drafts = JSON.parse(useRouter().query.drafts as string)
        
    if(useRouter().query.posts !== undefined)
        posts = JSON.parse(useRouter().query.posts as string)

    useEffect(() => {
        if(Router.query.drafts === undefined) getDrafts()
    })

    if(useSession().status === "unauthenticated") handleSignInNoEvent()

    return (
        <>
            <Head>
                <title>Create | Tome</title>
            </Head>
            <h1 className="text-3xl ml-2 mt-2">Create a post</h1>
            <h1 className="text-2xl ml-2 mt-2">Drafts</h1>
            <div id="drafts" className="">
                <span className="ml-2"><Button className="" onClick={createNewDraft}>Create a new draft</Button></span> <br />
                {drafts.reverse().map((value, index) => {
                    return <Draft title={value.title} body={value.body} thumbnail={value.thumbnail} id={value._id as ObjectId}></Draft>
                })}
            </div>
            <h1 className="text-2xl ml-2 mt-2">Posts</h1>
            <div id="posts" className="">
                {posts.reverse().map((value, index) => {
                    return <PostCard title={value.title} body={value.body} thumbnail={value.thumbnail} id={value._id as ObjectId} 
                        views={value.views} likes={value.likes} dislikes={value.dislikes}></PostCard>
                })}
            </div>
        </>
    )
}

export default Create