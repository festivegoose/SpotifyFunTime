import React, { Component } from 'react';
import FetchData from "../utilities/FetchData";

export class Welcome extends Component {

    constructor(props) {
        super(props);
        this.state = { displayName: '', followerCount: 0, images: [], loading: true };
    }

    componentDidMount() {
        this.populateUser();
    }

    render() {
        const { displayName, followerCount, images } = this.state;
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : 
            <div>
                {displayName}, Number of followers: {followerCount}
                <br />
                {images.map(image => (
                    <img src={image.url} />
                ))}
            </div>

        return (
            <div>
                <h1 id="tabelLabel" >Welcome!</h1>
                {contents}
            </div>
        );
    }

    async populateUser() {
        const data = await FetchData.get('api/v1/user');
        console.log(data);
        this.setState({ 
            displayName: data.displayName, 
            followerCount: data.followers.total, 
            images: data.images,
            loading: false 
        });
    }
}
