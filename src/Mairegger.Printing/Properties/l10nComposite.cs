// Copyright 2017-2022 Michael Mairegger
// 
// Licensed under the Apache License, Version 2.0 (the "License")
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Diagnostics.CodeAnalysis;
using System.Text;

#if NET8_0_OR_GREATER

namespace Mairegger.Printing.Properties;

[SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores")]
public class l10nComposite
{
    public static readonly CompositeFormat ColumnDimensionAttribute_ColumnDimensionAttribute__0__is_no_valid_column_dimension = CompositeFormat.Parse(l10n.ColumnDimensionAttribute_ColumnDimensionAttribute__0__is_no_valid_column_dimension);
    public static readonly CompositeFormat PageRange_PageRange__0__must_be_lower_or_equal_than__1_ = CompositeFormat.Parse(l10n.PageRange_PageRange__0__must_be_lower_or_equal_than__1_);
    public static readonly CompositeFormat PrintDimension_GetHeightFor__0__must_return_a_value_for__Get_1_____if___2___is_set_ = CompositeFormat.Parse(l10n.PrintDimension_GetHeightFor__0__must_return_a_value_for__Get_1_____if___2___is_set_);
}

#endif
