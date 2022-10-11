import { Sign } from "crypto";
import { NextPage } from "next";
import Link from "next/link";
import { Anchor } from './anchor';
// import styles from '../styles/Main.module.css'
import { Button } from './button';
import { SignIn } from './signin';
import { useSession } from 'next-auth/react';

export const NavHeader: NextPage = () => {
    return(
        <>
            <nav className="flex items-center flex-row justify-center align-center pt-1 pb-1 border-b-2 border-medium w-full bg-lightest">
                <span className="ml-2 w-full flex items-center flex-row justify-start align-center">
                   <Anchor href="/"><h1 className='text-3xl mr-2 border-r-2 border-medium pr-3 mt-0'>Tome</h1></Anchor>
                   <Anchor href="/discover" query={{mode: "explore"}}><h1 className='text-2xl mr-3 border-r-2 border-medium pr-2'>Explore</h1></Anchor>
                   <Anchor href="/discover" query={{mode: "top"}}><h1 className='text-2xl mr-3 border-r-2 border-medium pr-2'>Top</h1></Anchor>
                   <Anchor href="/discover" query={{mode: "hot"}}><h1 className='text-2xl mr-3 border-r-2 border-medium pr-2'>Hot</h1></Anchor>
                   <Anchor href="/discover" query={{mode: "recent"}}><h1 className='text-2xl mr-3 border-r-2 border-medium pr-2'>Recent</h1></Anchor>
                   <Anchor href="/about"><h1 className='text-2xl border-medium pr-2'>About</h1></Anchor>
                </span>
                <span className="flex items-center flex-row justify-start align-center">
                   <Anchor href="/create"><h1 className='text-2xl mr-3 border-r-2 border-medium pr-2'>Create</h1></Anchor>
                   {useSession().status !== "unauthenticated" && <Anchor href="/profile"><h1 className='text-2xl mr-3 border-r-2 border-medium pr-2'>Profile</h1></Anchor>}
                   <SignIn className=" mr-3 " textClassName={""} signOut={true} />
                </span>
            </nav>
        </>
    );
};