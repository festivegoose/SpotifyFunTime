async function get(url) {
    const response = await fetch(url);
    
    if (response.status === 401) {
        window.location.replace("/login");
    }
    
    const data = await response.json();
    return data.content;
}

export default {
    get
};