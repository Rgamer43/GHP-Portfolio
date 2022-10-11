import { NextPage } from "next";

const About: NextPage = () => {
    return (
        <>
            <div className="m-10 flex flew-row">
                <div className="w-2/3">
                    <b>Welcome to Tome!</b>
                </div>
                <div>
                    <iframe src="https://discord.com/widget?id=1026501765232869376&theme=dark" width="350" height="650" 
                        allowTransparency={true} frameBorder="1" 
                        sandbox="allow-popups allow-popups-to-escape-sandbox allow-same-origin allow-scripts" 
                        className="shadow-sm"></iframe>
                </div>
            </div>
        </>
    )
}

export default About