using Collage.WF.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Collections;
using System.Reflection;

namespace Collage.WF
{
    public partial class Form1 : Form
    {
        private EFAppContext _context = null!;
        private string _currentTable = "";
        private readonly HashSet<string> _identityKeyColumns = new(StringComparer.OrdinalIgnoreCase);

        private readonly Dictionary<string, Type> _tableMap = new()
        {
            { "Department", typeof(Department) },
            { "Instructor", typeof(Instructor) },
            { "Course", typeof(Course) },
            { "CourseSession", typeof(CourseSession) },
            { "Student", typeof(Student) },
            { "Course_Student", typeof(CourseStudent) },
            { "CourseSessionAttendance", typeof(CourseSessionAttendance) }
        };

        public Form1()
        {
            InitializeComponent();
            Load += Form1_Load;
            dgvData.CellFormatting += dgvData_CellFormatting;
            dgvData.DataError += dgvData_DataError;
        }

        private void Form1_Load(object? sender, EventArgs e)
        {
            _context = new EFAppContext();
            cmbTables.Items.AddRange([.. _tableMap.Keys]);
            cmbTables.SelectedIndex = 0;
        }

        private void cmbTables_SelectedIndexChanged(object? sender, EventArgs e)
        {
            _currentTable = cmbTables.SelectedItem?.ToString() ?? "";
            LoadTableData();
        }

        private void LoadTableData()
        {
            try
            {
                _context.Dispose();
                _context = new EFAppContext();

                switch (_currentTable)
                {
                    case "Department":
                        _context.Departments.Load();
                        dgvData.DataSource = _context.Departments.Local.ToBindingList();
                        break;
                    case "Instructor":
                        _context.Instructors.Load();
                        dgvData.DataSource = _context.Instructors.Local.ToBindingList();
                        break;
                    case "Course":
                        _context.Courses.Load();
                        dgvData.DataSource = _context.Courses.Local.ToBindingList();
                        break;
                    case "CourseSession":
                        _context.CourseSessions.Load();
                        dgvData.DataSource = _context.CourseSessions.Local.ToBindingList();
                        break;
                    case "Student":
                        _context.Students.Load();
                        dgvData.DataSource = _context.Students.Local.ToBindingList();
                        break;
                    case "Course_Student":
                        _context.CourseStudents.Load();
                        dgvData.DataSource = _context.CourseStudents.Local.ToBindingList();
                        break;
                    case "CourseSessionAttendance":
                        _context.CourseSessionAttendances.Load();
                        dgvData.DataSource = _context.CourseSessionAttendances.Local.ToBindingList();
                        break;
                }

                HideNavigationColumns();
                SetIdentityColumnsReadOnly();
                ConfigureForeignKeyComboColumns();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigureForeignKeyComboColumns()
        {
            if (string.IsNullOrWhiteSpace(_currentTable))
                return;

            if (!_tableMap.TryGetValue(_currentTable, out var entityType))
                return;

            var currentEntity = _context.Model.FindEntityType(entityType);
            if (currentEntity == null)
                return;

            foreach (var foreignKey in currentEntity.GetForeignKeys())
            {
                if (foreignKey.Properties.Count != 1 || foreignKey.PrincipalKey.Properties.Count != 1)
                    continue;

                var foreignKeyProperty = foreignKey.Properties[0];
                var existingColumn = dgvData.Columns[foreignKeyProperty.Name];
                if (existingColumn == null || existingColumn is DataGridViewComboBoxColumn)
                    continue;

                var lookupItems = BuildForeignKeyLookupItems(foreignKey.PrincipalEntityType, foreignKeyProperty.IsNullable);
                if (lookupItems.Count == 0)
                    continue;

                var index = existingColumn.Index;
                var headerText = existingColumn.HeaderText;
                var isVisible = existingColumn.Visible;
                var width = existingColumn.Width;

                dgvData.Columns.Remove(existingColumn);

                var comboColumn = new DataGridViewComboBoxColumn
                {
                    Name = foreignKeyProperty.Name,
                    DataPropertyName = foreignKeyProperty.Name,
                    HeaderText = headerText,
                    DataSource = lookupItems,
                    ValueMember = nameof(ForeignKeyLookupItem.Value),
                    DisplayMember = nameof(ForeignKeyLookupItem.Display),
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton,
                    FlatStyle = FlatStyle.Flat,
                    AutoComplete = true,
                    Visible = isVisible,
                    Width = width
                };

                dgvData.Columns.Insert(index, comboColumn);
            }
        }

        private List<ForeignKeyLookupItem> BuildForeignKeyLookupItems(IEntityType principalEntityType, bool includeEmpty)
        {
            var principalKey = principalEntityType.FindPrimaryKey();
            if (principalKey == null || principalKey.Properties.Count != 1)
                return [];

            var keyPropertyName = principalKey.Properties[0].Name;
            var keyProperty = principalEntityType.ClrType.GetProperty(keyPropertyName);
            if (keyProperty == null)
                return [];

            var rows = GetEntityRows(principalEntityType.ClrType);
            var items = new List<ForeignKeyLookupItem>();

            if (includeEmpty)
            {
                items.Add(new ForeignKeyLookupItem { Value = null, Display = string.Empty });
            }

            foreach (var row in rows)
            {
                var keyValue = keyProperty.GetValue(row);
                if (keyValue == null)
                    continue;

                items.Add(new ForeignKeyLookupItem
                {
                    Value = keyValue,
                    Display = GetDisplayValue(row, keyProperty)
                });
            }

            return items;
        }

        private List<object> GetEntityRows(Type clrType)
        {
            var setMethod = typeof(DbContext).GetMethods()
                .First(method => method.Name == nameof(DbContext.Set)
                    && method.IsGenericMethod
                    && method.GetParameters().Length == 0);

            var genericSetMethod = setMethod.MakeGenericMethod(clrType);
            var set = genericSetMethod.Invoke(_context, null);
            if (set is not IEnumerable rows)
                return [];

            return rows.Cast<object>().ToList();
        }

        private static string GetDisplayValue(object entity, PropertyInfo keyProperty)
        {
            var type = entity.GetType();

            var nameProperty = type.GetProperty("Name");
            if (nameProperty?.PropertyType == typeof(string))
            {
                var value = nameProperty.GetValue(entity)?.ToString();
                if (!string.IsNullOrWhiteSpace(value))
                    return value;
            }

            var titleProperty = type.GetProperty("Title");
            if (titleProperty?.PropertyType == typeof(string))
            {
                var value = titleProperty.GetValue(entity)?.ToString();
                if (!string.IsNullOrWhiteSpace(value))
                    return value;
            }

            var firstNameProperty = type.GetProperty("FirstName");
            var lastNameProperty = type.GetProperty("LastName");
            if (firstNameProperty?.PropertyType == typeof(string) && lastNameProperty?.PropertyType == typeof(string))
            {
                var first = firstNameProperty.GetValue(entity)?.ToString() ?? string.Empty;
                var last = lastNameProperty.GetValue(entity)?.ToString() ?? string.Empty;
                var fullName = $"{first} {last}".Trim();
                if (!string.IsNullOrWhiteSpace(fullName))
                    return fullName;
            }

            return keyProperty.GetValue(entity)?.ToString() ?? string.Empty;
        }

        private void dgvData_DataError(object? sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private sealed class ForeignKeyLookupItem
        {
            public object? Value { get; set; }
            public string Display { get; set; } = string.Empty;
        }

        private void HideNavigationColumns()
        {
            foreach (DataGridViewColumn col in dgvData.Columns)
            {
                var propType = col.ValueType;
                if (propType != null &&
                    !propType.IsPrimitive &&
                    propType != typeof(string) &&
                    propType != typeof(DateTime) &&
                    propType != typeof(decimal) &&
                    !propType.IsEnum &&
                    Nullable.GetUnderlyingType(propType) == null)
                {
                    col.Visible = false;
                }
            }
        }

        private void SetIdentityColumnsReadOnly()
        {
            _identityKeyColumns.Clear();

            if (string.IsNullOrWhiteSpace(_currentTable))
                return;

            if (!_tableMap.TryGetValue(_currentTable, out var entityType))
                return;

            var key = _context.Model.FindEntityType(entityType)?.FindPrimaryKey();
            if (key == null)
                return;

            foreach (var property in key.Properties)
            {
                var generationStrategy = property.FindAnnotation("SqlServer:ValueGenerationStrategy")?.Value?.ToString();
                if (!string.Equals(generationStrategy, "IdentityColumn", StringComparison.Ordinal))
                    continue;

                var column = dgvData.Columns[property.Name];
                if (column == null)
                    continue;

                column.ReadOnly = true;
                column.DefaultCellStyle.BackColor = Color.LightGray;
                _identityKeyColumns.Add(property.Name);
            }
        }

        private void dgvData_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            var row = dgvData.Rows[e.RowIndex];
            if (!row.IsNewRow)
                return;

            var column = dgvData.Columns[e.ColumnIndex];
            if (!_identityKeyColumns.Contains(column.Name))
                return;

            if (e.Value is int intValue && intValue == 0)
            {
                e.Value = string.Empty;
                e.FormattingApplied = true;
            }
        }

        private void btnAdd_Click(object? sender, EventArgs e)
        {
            if (dgvData.DataSource != null)
            {
                dgvData.AllowUserToAddRows = true;
                if (dgvData.Rows.Count > 0)
                {
                    var newRowIndex = dgvData.Rows.Count - 1;
                    var newRow = dgvData.Rows[newRowIndex];
                    dgvData.ClearSelection();
                    newRow.Selected = true;
                    dgvData.CurrentCell = newRow.Cells
                        .Cast<DataGridViewCell>()
                        .FirstOrDefault(cell => cell.Visible && !cell.ReadOnly)
                        ?? newRow.Cells[0];
                }
            }
        }

        private void btnDelete_Click(object? sender, EventArgs e)
        {
            if (dgvData.CurrentRow == null || dgvData.CurrentRow.IsNewRow)
            {
                MessageBox.Show("Please select a row to delete.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var result = MessageBox.Show("Are you sure you want to delete the selected row?",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    var row = dgvData.CurrentRow;
                    var item = row.DataBoundItem;
                    if (item != null)
                    {
                        _context.Remove(item);
                        _context.SaveChanges();
                        LoadTableData();
                        MessageBox.Show("Row deleted successfully.", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting row: {ex.InnerException?.Message ?? ex.Message}",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    LoadTableData();
                }
            }
        }

        private void btnSave_Click(object? sender, EventArgs e)
        {
            try
            {
                dgvData.EndEdit();
                _context.SaveChanges();
                MessageBox.Show("Changes saved successfully.", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadTableData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving changes: {ex.InnerException?.Message ?? ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LoadTableData();
            }
        }

        private void btnRefresh_Click(object? sender, EventArgs e)
        {
            LoadTableData();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _context?.Dispose();
            base.OnFormClosed(e);
        }
    }
}
