import { NextPage } from "next"
import Head from "next/head"
import { Button } from '../components/button';
import { createAccount } from "../lib/networkmanager";

const submit = async (event: any) => {
    event.preventDefault()
    if(event.target.username != "") {
        console.log("Formed submitted")
        console.log("Username: " + event.target.username.value)
        console.log("Bio: " + event.target.bio.value)
        createAccount(event.target.username.value, event.target.bio.value)
    } else {
        document.getElementById('error')!.innerHTML = "Must enter a username."
    }
}

const SignUp: NextPage = () => {
    return (
        <>
            <Head>
                <title>Sign Up | Tome</title>
            </Head>
            <main>
                <div className="grid place-items-center h-2/3 mt-36">
                    <h1 className="text-5xl border-b-2 pb-2 border-black">Sign Up</h1>
                    <form action="" className="mt-5 grid place-items-center w-[40%]" onSubmit={submit}>
                        <div className="mb-3 w-full">
                            <label htmlFor="username" className="text-xl">Username</label>
                            <input type="text" required={true} id="username" name="username" 
                               className="bg-lightest border-medium focus:border-dark text-xl border-2 rounded-md w-full pl-1"/>
                        </div>
                        <div className="mb-5 w-full">
                            <label htmlFor="bio" className="text-xl">Bio</label> <br />
                            <textarea required={false} id="bio" name="bio" 
                               className="bg-lightest border-medium focus:border-dark border-2 rounded-md pb-4 pl-1 w-full h-36"/>
                        </div>
                        <Button type='submit' className={"w-10 text-xl"}>Submit</Button>
                    </form>
                    <h3 id="error" className="mt-5 text-red-600"></h3>
                </div>
            </main>
        </>
    )
}

export default SignUp