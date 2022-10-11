import { ObjectId } from "mongodb";
import { NextPage } from "next"
import Image from 'next/image';
import { Button } from './button';
import { goToDraft } from '../lib/networkmanager';

type Props = {
    title: string,
    body: string,
    thumbnail: string,
    id: ObjectId
}

export const Draft: NextPage<Props> = (props) => {

    return (
        <>
            <Button className="bg-lightest m-2 max-w-xs max-h-96 p-1" onClick={() => {goToDraft(props.id)}}>
                <Image src={"/api/imageproxy?url=" + props.thumbnail} width="200px" height="125px" className="rounded-sm p-1 object-cover"/>
                <h1 className="text-lg">{props.title.slice(0,  process.env.CARD_TITLE_MAX as unknown as number)}</h1>
                <p className="text-sm">{props.body.length > (process.env.CARD_BODY_MAX as unknown as number) ? props.body.slice(0,  process.env.CARD_BODY_MAX as unknown as number) + "..." : props.body}</p>
            </Button>
        </>
    )
}