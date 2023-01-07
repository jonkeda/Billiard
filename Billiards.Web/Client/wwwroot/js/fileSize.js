export async function getImageDimensions(content) {
    const url = URL.createObjectURL(new Blob([await content.arrayBuffer()]));
    const dimensions = await new Promise(resolve => {
        const img = new Image();
        img.onload = function () {
            const data = { Width: img.naturalWidth, Height: img.naturalHeight };
            resolve(data);
        };
        img.src = url;
    });
    return JSON.stringify(dimensions);
}