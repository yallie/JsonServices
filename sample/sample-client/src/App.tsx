import * as React from 'react';
import './App.css';
import logo from './logo.svg';
import ServiceExecutor from './ServiceExecutor';

class App extends React.Component {
  public render() {
    return (
      <div className="App">
        <header className="App-header">
          <img src={logo} className="App-logo" alt="logo" />
          <h1 className="App-title">JsonServices React Demo</h1>
        </header>
        <ServiceExecutor />
      </div>
    );
  }
}

export default App;
