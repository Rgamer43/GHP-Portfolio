import { NextPage } from "next";
import Head from "next/head";
import { useRouter } from "next/router";
import { useEffect } from 'react';
import { Button } from "../components/button";
import { deleteDraft, getDraft, publishDraft, updateDraft } from '../lib/networkmanager';

const submit = async (event: any) => {
    event.preventDefault()

    console.log("Saving draft...")
    // console.log("Title: " + event.target.title)
    // console.log("Thumbnail: " + event.target.thumbnail)
    // console.log("Body: " + event.target.body)
    updateDraft({
        _id: useRouter().query.draftID,
        title: event.target.title.value,
        thumbnail: event.target.thumbnail.value,
        headerImage: event.target.headerImage.value,
        body: event.target.body.value
    })
}

const save = async () => {
    let title = (document.getElementById('title') as HTMLInputElement).value
    let thumbnail = (document.getElementById('thumbnail') as HTMLInputElement).value
    let headerImage = (document.getElementById('headerImage') as HTMLTextAreaElement).value
    let body = (document.getElementById('body') as HTMLTextAreaElement).value

    console.log("Saving draft...")
    // console.log("Title: " + title)
    // console.log("Thumbnail: " + thumbnail)
    // console.log("Body: " + body)
    updateDraft({
        _id: useRouter().query.draftID,
        title: title,
        thumbnail: thumbnail,
        headerImage: headerImage,
        body: body
    })
}

const saveEvent = (e: any) => {
    save()
}

const publish = async () => {
    let draft = {
        _id: useRouter().query.draftID,
        title: (document.getElementById("title") as HTMLInputElement).value,
        thumbnail: (document.getElementById("thumbnail") as HTMLInputElement).value,
        headerImage: (document.getElementById("headerImage") as HTMLInputElement).value,
        body: (document.getElementById("body") as HTMLTextAreaElement).value
    }

    if(confirm("Are you sure you want to publish?"))
        publishDraft(draft)
}

const deleteThisDraft = async () => {
    if(confirm("Are you sure you want to delete this draft? This cannot be undone!"))
        deleteDraft(useRouter().query.draftID as string)
}

const Write: NextPage = () => {
    let draftID: string | string[] | undefined
    draftID = useRouter().query.draftID

    useEffect(() => {
        if(draftID !== undefined)
            getDraft()
    })

    return (
        <>
            <Head>
                <title>Write | Tome</title>
            </Head>
            <main>
                <div className="grid place-items-center mt-4 h-full">   
                    <form action="" className="w-4/6 h-full" onSubmit={submit} id="editdraft">
                        <div className="mb-3">
                            <label htmlFor="title" className="text-xl">Title: </label>
                            <input type="text" required={true} id="title" name="title" onChange={saveEvent} 
                               className="bg-lightest border-medium focus:border-dark text-xl border-2 rounded-md w-full pl-1"/>
                        </div>
                        <div className="mb-3">
                            <label htmlFor="thumbnail" className="text-xl">Thumbnail Image URL: </label>
                            <input type="text" required={false} id="thumbnail" name="thumbnail" onChange={saveEvent} 
                               className="bg-lightest border-medium focus:border-dark text-xl border-2 rounded-md w-full pl-1"/>
                        </div>
                        <div className="mb-3">
                            <label htmlFor="headerImage" className="text-xl">Header Image URL: </label>
                            <input type="text" required={false} id="headerImage" name="headerImage" onChange={saveEvent} 
                               className="bg-lightest border-medium focus:border-dark text-xl border-2 rounded-md w-full pl-1"/>
                        </div>
                        <div className="mb-3">
                            <label htmlFor="body" className="text-xl">Content:</label> <br />
                            <textarea required={true} id="body" name="body" onChange={saveEvent} 
                               className="block bg-lightest border-medium focus:border-dark border-2 rounded-md pb-4 pl-1 w-full h-96"/>
                        </div>
                    </form>
                    <div className="align-middle items-start">
                        {
                            //<Button type='submit' className={"w-20 text-2xl p-1 mr-3"} onClick={save}>Save Draft</Button>
                        }
                        <Button className={"w-20 text-2xl p-1 mr-3"} onClick={publish}>Publish</Button>
                        <Button className={"w-20 text-2xl p-1 mr-3 bg-red-300 hover:bg-red-400 active:bg-red-500"} 
                            onClick={deleteThisDraft}>Delete Draft</Button>
                    </div>
                    <h3 id="error" className="mt-5 text-red-600"></h3>
                </div>
            </main>
        </>
    )
}

export default Write