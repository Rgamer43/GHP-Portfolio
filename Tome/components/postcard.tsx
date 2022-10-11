import { ObjectId } from "mongodb";
import { NextPage } from "next"
import Image from 'next/image';
import { Button } from './button';
import { goToDraft, goToPost } from '../lib/networkmanager';

type Props = {
    title: string,
    body: string,
    thumbnail: string,
    id: ObjectId,
    views: number,
    likes: number,
    dislikes: number,
    author?: string
}

export const PostCard: NextPage<Props> = (props) => {
    let views: any
    if(props.views === undefined) views = 0
    else views = props.views

    let likes: any
    if(props.likes === undefined) likes = 0
    else likes = props.likes

    let dislikes: any
    if(props.dislikes === undefined) dislikes = 0
    else dislikes = props.dislikes
    
    let author: any
    if(props.author === undefined) author = "you"
    else author = props.author

    return (
        <>
            <Button className="bg-lightest m-2 max-w-sm min-h-fit p-1" onClick={() => {goToPost(props.id)}}>
                <Image src={"/api/imageproxy?url=" + props.thumbnail} width="200px" height="125px" className="rounded-sm p-1 object-cover"/>
                <h1 className="text-lg">{props.title.slice(0, process.env.CARD_TITLE_MAX as unknown as number)}</h1>
                <p className=" text-sm">By {author}, Views: {views.length}, Likes: 
                    {(likes == 0 && dislikes == 0) ? " None" : " " + Math.round((likes.length/(dislikes.length+likes.length))*100) + "%"}</p>
                <p className="text-xs max-w-[15rem] flex flex-wrap align-middle">{props.body.length > (process.env.CARD_BODY_MAX as unknown as number) ? props.body.slice(0, process.env.CARD_BODY_MAX as unknown as number) + "..." : props.body}</p>
            </Button>
        </>
    )
}