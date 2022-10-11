import { NextPage } from "next"
import { isAdmin, socket } from "../lib/networkmanager";
import { Anchor } from './anchor';
import { Button } from "./button";

type Props = {
    user: string,
    text: string,
    time: string,
    post: string,
    id: number,
    admin: boolean
}

const deleteThis = (post: string, id: number) => {
    if(confirm("Are you sure you want to delete this comment?") === true) socket.emit("deletecomment", [post, id])
}

export const Comment: NextPage<Props> = (props: Props) => {
    return (
        <>
            <div className="bg-lighter rounded-md mb-2 p-1">
                <div className="flex flex-row items-baseline">
                    <Anchor className="text-lg" href={"profile"} query={{user: props.user}}><h3>{props.user}</h3></Anchor>
                    <p className="text-xs ml-1">{props.time}</p>
                    {props.admin && <Button onClick={()=>{deleteThis(props.post, props.id)}} className="ml-1 border-2">Delete</Button>}
                </div>
                <p className="text-sm">{props.text}</p>
            </div>
        </>
    )
}