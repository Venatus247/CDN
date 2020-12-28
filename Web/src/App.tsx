import React, { useState } from 'react'
import {BrowserRouter as Router, Switch, Link, Route} from 'react-router-dom'

import Home from "./pages/Home"
import Login from "./pages/Login"

import './App.sass'

function App() {
  return (
    <div className="App">
      <Router>
          <Switch>
              <Route path="/" exact component={Home}/>
              <Route path="/login" component={Login}/>
          </Switch>
      </Router>
    </div>
  )
}

export default App
