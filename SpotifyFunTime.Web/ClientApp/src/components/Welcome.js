import React, { Component } from 'react';

export class Welcome extends Component {

    constructor(props) {
        super(props);
        this.state = { displayName: '', loading: true };
    }

    componentDidMount() {
        this.populateUser();
    }

    static renderUserInfo(displayName) {
        return (
            <div>
                {displayName}
            </div>
        );
    }

    render() {
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : Welcome.renderUserInfo(this.state.displayName);

        return (
            <div>
                <h1 id="tabelLabel" >Welcome!</h1>
                {contents}
            </div>
        );
    }

    async populateUser() {
        const response = await fetch('api/v1/spotify/welcome');
        const data = await response.json();
        const content = data.content;
        this.setState({ displayName: content.displayName, loading: false });
    }
}
