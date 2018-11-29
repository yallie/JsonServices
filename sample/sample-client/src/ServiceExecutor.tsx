import * as React from 'react';
import JsonClient from './JsonServices/JsonClient';
import { GetVersion } from './Messages/GetVersion';

interface IState {
    messageLog: string;
    version: string;
    versionIsInternal: boolean;
    webSocketAddress: string;
    webSocketStatus: string;
}

export class ServiceExecutor extends React.Component<{}, IState> {
    constructor(props: {}) {
        super(props);
        this.state = {
            webSocketAddress: "ws://localhost:8765/", //  window.location.toString().replace("http", "ws"),
            webSocketStatus: 'Not connected',
            versionIsInternal: true,
            messageLog: '',
            version: '(not called)',
        };
    }

    private client?: JsonClient;

    private connect = async () => {
        try {
            this.client = new JsonClient(this.state.webSocketAddress);
            await this.client.connectAsync();
            this.client.traceMessage = this.traceMessage;
            this.setState({
                webSocketStatus: 'Connected'
            });
        } catch (e) {
            this.client = undefined;
            this.setState({
                webSocketStatus: 'Failure: ' + e.message
            })
        }
    }

    private traceMessage = (e: { isOutcoming: boolean, data: string }) => {
        this.setState(state => ({
            messageLog: state.messageLog + (e.isOutcoming ? "--> " : "<-- ") + e.data + "\n"
        }));
    }

    private getVersion = async () => {
        if (this.client === undefined) {
            this.setState({
                version: 'not connected! try connecting first'
            });
            return;
        }

        try {
            const getVersion = new GetVersion();
            getVersion.IsInternal = this.state.versionIsInternal;
            const result = await this.client.call(getVersion);
            this.setState({
                version: result.Version
            })
        } catch (e) {
            this.setState({
                version: 'error: ' + e.message
            })
        }
    }

    private editWebSocketAddress = (e: React.ChangeEvent<HTMLInputElement>) => {
        this.setState({
            webSocketAddress: e.currentTarget.value
        });
    }

    private toggleVersionIsInternal = (e: React.ChangeEvent<HTMLInputElement>) => {
        this.setState(o => ({
            versionIsInternal: !o.versionIsInternal
        }));
    }

    public render() {
        return (
            <div>
                <h1>Server address</h1>
                <input type="text" value={this.state.webSocketAddress} onChange={this.editWebSocketAddress} />
                <input type="button" value="Connect" onClick={this.connect} style={{ margin: 8 }}/>
                <span>{this.state.webSocketStatus}</span>

                <h1>GetVersion</h1>
                <label style={{ marginRight: 8 }}>
                    <input type="checkbox" checked={this.state.versionIsInternal} onChange={this.toggleVersionIsInternal}/>
                    IsInternal
                </label>
                <input type="button" value="GetVersion" onClick={this.getVersion} />
                <label style={{ marginLeft: 8 }}>
                    Result: {this.state.version}
                </label>

                <h1>Message log</h1>
                <pre>{this.state.messageLog}</pre>
            </div>
        );
    }
}

export default ServiceExecutor;