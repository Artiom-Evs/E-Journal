import { Component } from "react"

export class Form extends Component {
    constructor(props) {
        super(props);

        this.handleChange = this.handleChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
        this.handleCancel = this.handleCancel.bind(this);

        this.state = {
            ...this.buildDefaultStateProps(props.properties, props.item)
        };
    }

    buildDefaultStateProps(props, item) {
        let build = {};
        props.forEach(p => {
            switch (p.type) {
                case 'text':
                    build[p.id] = item ? item[p.id] : '';
                    break;
                case 'number':
                    build[p.id] = item ? item[p.id] : 0;
                    break;
                case 'select':
                    let defaultValue = p.default ? p.default : '';
                    build[p.id] = item ? item[p.id] : defaultValue;
                    break;
                case 'hidden':
                    build[p.id] = item ? item[p.id] : '0';;
                    break;
                default:
                    console.error(`Unsupported form component type: ${p.type}`);
            }
        });
        return build;
    }

    setItemProps() {
        let model = {};
        this.props.properties.forEach(p => {
            model[p.id] = this.props.item[p.id];
        });
        this.setState(model);
    }

    buildModel() {
        let model = {};
        this.props.properties.forEach(p => {
            model[p.id] = this.state[p.id];
        });
        return model;
    }

    validateInput(propName, value) {
        let prop = this.props.properties.find(p => p.id === propName);
        let isValid = this.validateProperty(prop, value);
        document.getElementById(`${prop.id}Message`).innerText = isValid ? '' : `"${prop.name}" является обязательным полем`;
        return isValid;
    }

    validateProperty(prop, value) {
        let isRequired = prop.required === undefined ? true : prop.required;
        let defaultValue = prop.default === undefined ? '' : prop.default;
        value = value === undefined ? this.state[prop.id] : value;
        
        if (!isRequired) return true;
        if (value === defaultValue) return false;
        return !!value;
    }

    handleChange(event) {
        let state = {};
        state[event.target.name] = event.target.value;
        this.setState(state);
        this.validateInput(event.target.name, event.target.value);
    }

    handleSubmit(event) {
        event.preventDefault();

        let isValid = true;
        this.props.properties.forEach(p => {
            if (p.type === 'hidden') return;
            if (!this.validateInput(p.id)) isValid = false;
        });
        if (!isValid) return;

        let model = this.buildModel();
        this.props.onSubmit(model);

    }

    handleCancel(event) {
        this.props.onCancel();
        event.preventDefault();
    }

    renderFormComponent(prop) {
        switch (prop.type) {
            case 'text':
                return this.renderTextInput(prop);
            case 'number':
                return this.renderNumberInput(prop);
            case 'select':
                return this.renderSelectInput(prop);
            case 'hidden':
                return null;
            default:
                console.error(`Unsupported form component type: ${prop.type}`);
        }
    }

    renderTextInput(prop) {
        return (
            <div className='form-group'>
                <label htmlFor={`${prop.id}Input`}>{prop.name}</label>
                <input className="form-control" type='text' id={`${prop.id}Input`} name={prop.id} value={this.state[prop.id]} placeholder={prop.placeholder ? prop.placeholder : ''} onChange={this.handleChange} />
                <small id={`${prop.id}Message`} className="form-text text-muted"></small>
            </div>
        )
    }

    renderNumberInput(prop) {
        return (
            <div className='form-group'>
                <label htmlFor={`${prop.id}Input`}>{prop.name}</label>
                <input className="form-control" type='number' min='0' max='7' id={`${prop.id}Input`} name={prop.id} value={this.state[prop.id]} placeholder={prop.placeholder ? prop.placeholder : ''} onChange={this.handleChange} />
                <small id={`${prop.id}Message`} className="form-text text-muted"></small>
            </div>
        )
    }

    renderSelectInput(prop) {
        return (
            <div className='form-group'>
                <label htmlFor={`${prop.id}Input`}>{prop.name}</label>
                <select className="form-control" id={`${prop.id}Input`} name={prop.id} value={this.state[prop.id]} onChange={this.handleChange}>
                    <option value={prop.default ? prop.default : ''}></option>
                    {prop.options.map(o =>
                        <option key={o.key} value={o.key}>{o.value}</option>
                    )}
                </select>
                <small id={`${prop.id}Message`} className="form-text text-muted"></small>
            </div>
        );
    }

    render() {
        return (
            <form onSubmit={this.handleSubmit}>
                {this.props.properties.map(p =>
                    <div key={p.id}>
                        {this.renderFormComponent(p)}
                    </div>
                )}
                <input type="submit" className='btn btn-primary' value={this.props.submitText} />
                <input type="button" className='btn btn-secondary' onClick={this.handleCancel} value="Отмена" />
            </form>
        );
    }
}
