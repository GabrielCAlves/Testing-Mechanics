# HDRP → URP Converter

Convert HDRP materials to URP in bulk — with one click.

## Requirements

- Unity 2020.3 or later
- Universal Render Pipeline (URP) package installed

## Quick Start

1. Open **Tools → HDRP → URP Converter (Free)**
2. Drag materials or folders into the drop zone, or click **Find Broken** to auto-detect HDRP/broken materials in your project
3. Choose **Override** (edit in place) or **Save Copy** (writes to a `ConvertedMaterials/` subfolder)
4. Click **▶ Convert**

## Features

| Feature | Detail |
|---------|--------|
| Batch conversion | Drag folders; processes all materials recursively |
| Find Broken | Scans the entire project for HDRP and `InternalErrorShader` materials |
| Save Copy | Non-destructive; originals untouched |
| Undo support | Full Unity Undo/Redo integration |
| Scene Lighting Normalization | Converts physical HDRP light intensities to URP-compatible values and clears baked lightmap references |

## What Gets Converted

- Base color + texture
- Normal map + scale
- Mask map (Metallic / Smoothness / Occlusion packed)
- Metallic & Smoothness remap ranges
- Emissive color + texture (HDR normalized to LDR)
- Surface type: Opaque / Transparent / Cutout
- Unlit materials → URP Unlit shader

## Scene Lighting Normalization

HDRP uses physical light units (thousands of Lux/Candela). This tool patches the `.unity` file directly to bring intensities into a URP-friendly range:

- **Directional** lights above 10 → normalized to 1.5
- **Point / Spot** lights above 5 → clamped to `intensity / 100` in [0.5, 3]
- Clears all baked lightmap indices and scale offsets
- Sets ambient mode to flat (0.2, 0.2, 0.22)

> Back up your scene before running this feature. Rebake lighting after conversion.

## Known Limitations

- MaskMap channel packing (HDRP: R=Metallic, G=AO, B=Detail, A=Smoothness) is transferred as-is. URP expects R=Metallic, A=Smoothness — results may need manual adjustment for AO.
- Detail maps and subsurface scattering are not converted.
- Decal and Hair shaders are not supported.

## Support

For bug reports and questions, contact: [your email or support URL]
