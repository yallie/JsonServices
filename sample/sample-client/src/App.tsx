import React from 'react'
import './App.css'
import ServiceExecutor from './ServiceExecutor'

const App: React.FC = () => {
    return (
        <div className="App">
            <h1>JsonServices React Demo</h1>
            <ServiceExecutor />
        </div>
    )
}

export default App
