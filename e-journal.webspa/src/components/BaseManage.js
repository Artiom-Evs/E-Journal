
import { Component } from "react";
import { Table } from "./Table";
import { Form } from "./Form";

export const States = {
    Loading: 0,
    Table: 1,
    Create: 2,
    Edit: 3
};

export class BaseManage extends Component {
    _id = '';
    _properties = [];
    _h1Text = '';
    _createButtonText = '';
    _createH2Text = '';
    _editH2Text = '';

    constructor(props) {
        super(props);

        this.handleCreate = this.handleCreate.bind(this);
        this.handleUpdate = this.handleUpdate.bind(this);
        this.handleDelete = this.handleDelete.bind(this);

        this.state = {
            pageState: States.Table,
            items: [],
            selectedItem: null
        };
    }

    handleCreate(item) {
    }

    handleUpdate(item) {
    }

    handleDelete(item) {
    }

    renderLoading() {
        return (
            <em>Загрузка...</em>
        );
    }

    renderTable() {
        return (
            <div>
                <Table key={this.props.id}
                    properties={this._properties}
                    items={this.state.items} 
                    createButtonText={this._createButtonText}
                    onCreate={() => this.setState({pageState:States.Create})} 
                    onEdit={(item) => this.setState({ pageState: States.Edit, selectedItem: item })}
                    onDelete={this.handleDelete} />
            </div>
            );
    }

    renderCreate() {
        return (
            <div>
                <h2>{this._createH2Text}</h2>
                <Form key={`${this.props.id}Create`}
                    submitText='Создать'
                    properties={this._properties} 
                    onSubmit={this.handleCreate}
                    onCancel={() => this.setState({pageState:States.Table})} />
            </div>
            );
    }

    renderEdit() {
        return (
            <div>
                <h2>{this._editH2Text}</h2>
                <Form key={`${this.props.id}Create`}
                    submitText='Сохранить'
                    properties={this._properties} 
                    item={this.state.selectedItem}
                    onSubmit={this.handleUpdate}
                    onCancel={() => this.setState({pageState:States.Table})} />
            </div>
            );
    }
    
    render() {
        let {pageState} = this.state;
        let content = 'NULL';

        switch (pageState) {
            case States.Loading:
                content = this.renderLoading();
                break;
            case States.Table:
                content = this.renderTable();
                break;
            case States.Create:
                content = this.renderCreate();
                break;
            case States.Edit:
                content = this.renderEdit();
                break;
            default:
                console.error(`Unsupported page state type: ${pageState}`)
        }

        return (
            <div>
                <h1>{this._h1Text}</h1>
                {content}
            </div>
        );
    }
}