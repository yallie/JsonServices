import * as React from 'react';
import './App.css';
import ServiceExecutor from './ServiceExecutor';

/*
import logo from './logo.svg';
    <header className="App-header">
        <img src={logo} className="App-logo" alt="logo" />
        <h1 className="App-title">JsonServices React Demo</h1>
    </header>
*/

class App extends React.Component {
  public render() {
    return (
      <div className="App">
        <h1>JsonServices React Demo</h1>
        <ServiceExecutor />
      </div>
    );
  }
}

export default App;
