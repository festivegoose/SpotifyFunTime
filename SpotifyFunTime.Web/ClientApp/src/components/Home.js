import React, { Component } from 'react';

export class Home extends Component {
  render () {
    return (
      <div>
          <div>Let's get started!</div>
          <a href='/authorize'>Login to Spotify</a>
      </div>
    );
  }
}
