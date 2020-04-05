import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { Login } from './components/Login';
import { Counter } from './components/Counter';
import {Welcome} from "./components/Welcome";

import './custom.css'

export default class App extends Component {
  static displayName = App.name;

  render () {
    return (
      <Layout>
        <Route path='/login' component={Login} />
          <Route path='/welcome' component={Welcome} />
        <Route path='/counter' component={Counter} />
      </Layout>
    );
  }
}
