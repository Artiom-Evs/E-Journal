
import { Component } from "react";

export class Table extends Component {
    constructor(props) {
        super(props);

        this.handleCreate = this.handleCreate.bind(this);
        this.handleEdit = this.handleEdit.bind(this);
        this.handleDelete = this.handleDelete.bind(this);
    }

    handleCreate() {
        this.props.onCreate();
    }

    handleEdit(item) {
        this.props.onEdit(item);
    }

    handleDelete(item) {
        this.props.onDelete(item);
    }

    viewProperty(prop, value) {
        return prop.viewer ? prop.viewer(prop, value) : value;
    }

    render() {
        return (
            <div>
            <table className='table'>
                <thead>
                    <tr>
                        {this.props.properties.map((p, i) =>
                            <th key={i}>{p.name}</th>
                        )}
                        <th></th>
                    </tr>
                </thead>
                <tbody>
                    {this.props.items.map((item, i) =>
                        <tr key={i}>
                            {this.props.properties.map((p, i) =>
                                <td key={i}>{this.viewProperty(p, item[p.id])}</td>
                            )}
                            <td>
                                <button className="btn btn-secondary" onClick={() => this.handleEdit(item)}>Изменить</button>
                                <button className="btn btn-danger" onClick={() => this.handleDelete(item)}> Удалить</button>
                            </td>
                        </tr>
                    )}
                </tbody>
            </table >
            <button className="btn btn-primary" onClick={this.handleCreate}>{this.props.createButtonText}</button>
            </div>
        );
    }
}