function isImage(url: string) {
    const endings = ["jpg", "jpeg", "png", "webp", "avif", "gif", "svg"]

    for (let i = 0; i < endings.length; i++) {
        const element = endings[i];
        // console.log(element + " " + url.endsWith("." + element))
        if(url.endsWith("." + element))
            return true
    }

    return false
}

export default async (req: { query: { url: string; }; }, res: any) => {
    try {
        let url = decodeURIComponent(req.query.url);
        // console.log(url)
        if(!isImage(url))
            url = process.env.DEFAULT_THUMBNAIL!
        // console.log(url)
        
        const result = await fetch(url);
        const body: any = result.body;
        body.pipe(res);
    } catch {
        
    }
};