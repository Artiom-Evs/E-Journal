import { Component } from "react";
import authService from './api-authorization/AuthorizeService'

export class AuthInfo extends Component {
    constructor(props) {
        super(props);

        this.state = {
            user: null
        };
    }

    componentDidMount() {
        this.loadData();
    }

    async loadData() {
        let user = await authService.getUser();
        this.setState({
            user: user
        });
    }


    render() {
        let { user } = this.state;

        return (
            <div>
                <h1>Данные авторизации</h1>
                {JSON.stringify(user)}
            </div>
        );
    }    
}