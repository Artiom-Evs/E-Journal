import { Component } from 'react'
import { GroupsClient } from './api-clients/JournalApiClient';

class GroupForm extends Component {
    constructor(props) {
        super(props);

        this.handleChange = this.handleChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
        this.handleCancel = this.handleCancel.bind(this);

        let id = props.groupId ? props.groupId : 0;
        let name = props.groupName ? props.groupName : '';

        this.state = { value: name, id: id };

        this.onSubmit = props.onSubmit;
        this.onCancel = props.onCancel;
    }

    handleChange(event) {
        this.setState({ value: event.target.value });
    }

    handleSubmit(event) {
        this.onSubmit({
            id: this.state.id,
            name: this.state.value
        });
        event.preventDefault();
    }

    handleCancel(event) {
        this.onCancel();
        event.preventDefault();
    }

    render() {
        return (
            <div>
                <h2>{this.props.h2Text}</h2>
                <form onSubmit={this.handleSubmit}>
                    <div className='form-group'>
                        <label htmlFor='inputName'>Наименование группы</label>
                        <input name='nameOfGroup' type="text" className='form-control' id='inputName' placeholder='Введите наименование группы' value={this.state.value} onChange={this.handleChange} />
                    </div>
                    <input type="submit" className='btn btn-primary' value={this.props.submitText} />
                    <input type="button" className='btn btn-secondary' onClick={this.handleCancel} value="Отмена" />
                </form>
            </div>
        );
    }
}

const States = {
    Loading: 0,
    Table: 1,
    Create: 2,
    Update: 3
};

export class ManageGroups extends Component {
    _client = null;

    constructor(props) {
        super(props);

        this.createGroup = this.createGroup.bind(this);
        this.updateGroup = this.updateGroup.bind(this);

        this._client = new GroupsClient();
        this.state = {
            pageState: States.Loading,
            groups: [],
            selectedId: 0
        };
    }

    componentDidMount() {
        this.loadData();
    }

    onCreate() {
        this.setState({ pageState: States.Create });
    }

    onEdit(groupId) {
        this.setState({ 
            selectedId: groupId,
            pageState: States.Update
        });
    }

    async onDelete(groupId) {
        let response = await this._client.Delete(groupId);
        console.log(`=> Result: ${response.statusText}`);
        this.loadData();
    }

    async createGroup(group) {
        let response = await this._client.Post(group);
        console.log(`=> Result: ${response.statusText}`);
        this.loadData();
    }

    async updateGroup(group) {
        let response = await this._client.Put(group);
        console.log(`=> Result: ${response.statusText}`);
        this.loadData();
    }

    renderTable(groups) {
        return (
            <table className='table'>
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Наименование</th>
                        <th></th>
                    </tr>
                </thead>
                <tbody>
                    {groups.map(g =>
                        <tr key={g.id}>
                            <td>{g.id}</td>
                            <td>{g.name}</td>
                            <td>
                                <button className="btn btn-secondary" onClick={() => this.onEdit(g.id)}>Изменить</button>
                                <button className="btn btn-danger" onClick={() => this.onDelete(g.id)}> Удалить</button>
                            </td>
                        </tr>
                    )}
                </tbody>
                <tfoot>
                    <tr>
                        <td>
                            <button className="btn btn-primary" onClick={() => this.onCreate()}>Создать группу</button>
                        </td>
                    </tr>
                </tfoot>
            </table >
        );
    }

    render() {
        let { groups, pageState } = this.state;

        let content = "NULL";

        if (pageState === States.Loading) content = (<em>Загрузка...</em>);
        else if (pageState === States.Table) {
            content = (
                <div>
                    <h2>Список групп</h2>
                    {this.renderTable(groups)}
                </div>
            );
        }
        else if (pageState === States.Create) {
            content = (
                <GroupForm key='create'
                    submitText='Создать'
                    h2Text='Создание новой группы'
                    onSubmit={this.createGroup}
                    onCancel={() => this.setState({ pageState: States.Table })} />
            );
        }
        else if (pageState === States.Update) {
            let group = this.state.groups.find(g => g.id === this.state.selectedId);
            content = (
                <GroupForm key='update'
                    submitText='Сохранить'
                    h2Text='Редактирование группы'
                    groupId={group.id}
                    groupName={group.name}
                    onSubmit={this.updateGroup }
                    onCancel={() => this.setState({ pageState: States.Table })} />
            );
        }

        return (
            <div>
                <h1>Учебные группы</h1>
                {content}
            </div>
        );
    }

    async loadData() {
        this.setState({ pageState: States.Loading });
        let data = await this._client.Get();
        this.setState({ groups: data, pageState: States.Table });
    }
}